[CmdletBinding()]
param(
    [string]$ApiBaseUrl = "http://localhost:8080",
    [Parameter(Mandatory = $true)]
    [string]$Email,
    [Parameter(Mandatory = $true)]
    [string]$Password,
    [string]$Tenant = "root",
    [ValidateSet("admin", "dashboard")]
    [string]$App = "admin",
    [switch]$SkipDatabaseCheck,
    [string]$PostgresContainer = ""
)

$ErrorActionPreference = "Stop"
$base = $ApiBaseUrl.TrimEnd('/')
$commonHeaders = @{
    "Accept" = "application/json"
    "tenant" = $Tenant
    "X-FSH-App" = $App
}

function Write-Step([string]$Message) {
    Write-Host "`n[TEST] $Message" -ForegroundColor Cyan
}

function Write-Pass([string]$Message) {
    Write-Host "[PASS] $Message" -ForegroundColor Green
}

function Write-Fail([string]$Message) {
    Write-Host "[FAIL] $Message" -ForegroundColor Red
}

function Get-ErrorBody($ErrorRecord) {
    try {
        if ($null -ne $ErrorRecord.Exception.Response) {
            $reader = [System.IO.StreamReader]::new($ErrorRecord.Exception.Response.GetResponseStream())
            return $reader.ReadToEnd()
        }
    } catch { }
    return $ErrorRecord.Exception.Message
}

function Invoke-Json([string]$Method, [string]$Uri, $Body = $null, [hashtable]$Headers = $commonHeaders) {
    $params = @{
        Method = $Method
        Uri = $Uri
        Headers = $Headers
        ContentType = "application/json"
        UseBasicParsing = $true
    }
    if ($null -ne $Body) {
        $params.Body = ($Body | ConvertTo-Json -Depth 10)
    }
    return Invoke-WebRequest @params
}

Write-Host "DNA Cases smoke test" -ForegroundColor Yellow
Write-Host "API: $base"
Write-Host "Tenant: $Tenant | App: $App"

Write-Step "Checking API reachability"
try {
    $reachability = Invoke-WebRequest -Method GET -Uri "$base/health" -Headers @{ Accept = "application/json" } -UseBasicParsing
    if ($reachability.StatusCode -notin @(200, 204)) { throw "Expected 200 or 204, received $($reachability.StatusCode)" }
    Write-Pass "API is reachable"
} catch {
    Write-Host "Health endpoint was not available; continuing to token issuance because deployments may expose a different health route." -ForegroundColor Yellow
}

Write-Step "Issuing a fresh JWT from the same API"
$loginBody = @{
    email = $Email
    password = $Password
    twoFactorCode = $null
}
try {
    $tokenResponse = Invoke-Json -Method POST -Uri "$base/api/v1/identity/token/issue" -Body $loginBody -Headers $commonHeaders
    $tokenJson = $tokenResponse.Content | ConvertFrom-Json
    if ([string]::IsNullOrWhiteSpace($tokenJson.accessToken)) { throw "The response did not contain accessToken." }
    $accessToken = [string]$tokenJson.accessToken
    $authHeaders = @{
        "Accept" = "application/json"
        "Authorization" = "Bearer $accessToken"
        "tenant" = $Tenant
        "X-FSH-App" = $App
    }
    Write-Pass "JWT issued by the same API instance"
} catch {
    Write-Fail "JWT issuance failed: $(Get-ErrorBody $_)"
    Write-Host "Use the same API base URL for both token issuance and Cases requests." -ForegroundColor Yellow
    exit 20
}

Write-Host "[SKIP] DNA status placeholder check skipped; Cases endpoints are the verification target." -ForegroundColor Yellow
Write-Step "Listing existing DNA cases"
try {
    $listResponse = Invoke-Json -Method GET -Uri "$base/api/v1/dna/cases" -Headers $authHeaders
    if ($listResponse.StatusCode -ne 200) { throw "Expected 200, received $($listResponse.StatusCode)" }
    $listJson = $listResponse.Content | ConvertFrom-Json
    $beforeCount = @($listJson).Count
    Write-Pass "ListCases succeeded; returned $beforeCount item(s)"
} catch {
    Write-Fail "ListCases failed: $(Get-ErrorBody $_)"
    exit 40
}

$caseNumber = "DNA-SMOKE-$([DateTime]::UtcNow.ToString('yyyyMMddHHmmss'))"
$caseBody = @{
    caseNumber = $caseNumber
    title = "DNA smoke test"
    description = "Automated API verification"
}

Write-Step "Creating a DNA case ($caseNumber)"
try {
    $createResponse = Invoke-Json -Method POST -Uri "$base/api/v1/dna/cases" -Body $caseBody -Headers $authHeaders
    if ($createResponse.StatusCode -notin @(200, 201)) { throw "Expected 200 or 201, received $($createResponse.StatusCode)" }
    $createdJson = $createResponse.Content | ConvertFrom-Json
    Write-Pass "CreateCase succeeded"
} catch {
    Write-Fail "CreateCase failed: $(Get-ErrorBody $_)"
    Write-Host "If this is 403, the user lacks the DNA Cases.Create permission; this is an authorization assignment issue, not a route or migration issue." -ForegroundColor Yellow
    exit 50
}

Write-Step "Verifying the created case appears in the list"
try {
    $afterResponse = Invoke-Json -Method GET -Uri "$base/api/v1/dna/cases" -Headers $authHeaders
    $afterJson = @($afterResponse.Content | ConvertFrom-Json)
    $match = @($afterJson | Where-Object { $_.caseNumber -eq $caseNumber })
    if ($match.Count -ne 1) { throw "Created case was not found in ListCases response." }
    Write-Pass "Created case is visible in the same tenant list"
} catch {
    Write-Fail "Post-create list verification failed: $(Get-ErrorBody $_)"
    exit 60
}

if (-not $SkipDatabaseCheck) {
    Write-Step "Checking dna.Cases in PostgreSQL"
    if ([string]::IsNullOrWhiteSpace($PostgresContainer)) {
        $PostgresContainer = (docker ps --filter "name=postgres-" --format "{{.Names}}" | Select-Object -First 1)
    }
    if ([string]::IsNullOrWhiteSpace($PostgresContainer)) {
        Write-Fail "No running Aspire PostgreSQL container was found. Use -SkipDatabaseCheck or start AppHost."
        exit 70
    }
    try {
        $password = (docker inspect $PostgresContainer --format '{{range .Config.Env}}{{println .}}{{end}}' | Select-String '^POSTGRES_PASSWORD=').ToString().Split('=', 2)[1]
        $sql = "SELECT table_schema || '.' || table_name FROM information_schema.tables WHERE table_schema = 'dna' AND table_name = 'Cases';"
        $actualTable = $dbOutput.Trim(); if ($actualTable -ne 'dna.Cases') { throw ("Expected dna.Cases, received: " + $actualTable) }
        Write-Pass "PostgreSQL contains dna.Cases"
    } catch {
        Write-Fail "Database verification failed: $($_.Exception.Message)"
        exit 70
    }
}

Write-Host "`nRESULT: DNA Cases smoke test passed." -ForegroundColor Green
exit 0

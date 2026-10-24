param(
    [string]$BaselineVersion = '4.0.123',
    [Parameter(Mandatory)][string]$CandidateVersion,
    [ValidateRange(3, 100)][int]$Samples = 15,
    [string]$OutputPath
)

$ErrorActionPreference = 'Stop'
$project = Join-Path $PSScriptRoot 'Soenneker.Lepton.Suite.Benchmarks.csproj'
$outputRoot = Join-Path ([IO.Path]::GetTempPath()) "lepton-benchmarks-$([Guid]::NewGuid())"
$versions = @{ baseline = $BaselineVersion; candidate = $CandidateVersion }
foreach ($variant in @('baseline', 'candidate')) {
    $destination = Join-Path $outputRoot $variant
    dotnet build $project -c Release "-p:LeptonPackageVersion=$($versions[$variant])" -o $destination
    if ($LASTEXITCODE -ne 0) { throw "$variant benchmark build failed." }
}

$results = [Collections.Generic.List[object]]::new()
foreach ($shape in @('small', 'wide')) {
    foreach ($phase in @('cold', 'warm')) {
        for ($sample = 0; $sample -lt $Samples; $sample++) {
            # Alternate order to reduce systematic temperature/load bias. Every sample gets a fresh process.
            $variants = if ($sample % 2 -eq 0) { @('baseline', 'candidate') } else { @('candidate', 'baseline') }
            $modes = if ($sample % 2 -eq 0) { @('generated', 'framework') } else { @('framework', 'generated') }
            foreach ($variant in $variants) {
                $assembly = Join-Path $outputRoot "$variant/Soenneker.Lepton.Suite.Benchmarks.dll"
                foreach ($mode in $modes) {
                    $json = & dotnet $assembly $mode $phase $shape
                    if ($LASTEXITCODE -ne 0) { throw "$variant $mode $phase $shape failed." }
                    $result = $json | ConvertFrom-Json
                    $result | Add-Member variant $variant
                    $result | Add-Member version $versions[$variant]
                    $results.Add($result)
                }
            }
        }
    }
}
if ($OutputPath) { $results | ConvertTo-Json | Set-Content -LiteralPath $OutputPath }
$results | Group-Object variant, mode, phase, parameters | ForEach-Object {
    $times = $_.Group.nanosecondsPerBinding | Sort-Object
    $allocations = $_.Group.bytesPerBinding | Sort-Object
    [pscustomobject]@{
        Case = $_.Name
        Samples = $times.Count
        MedianNanoseconds = $times[[int][Math]::Floor($times.Count / 2)]
        MedianBytes = $allocations[[int][Math]::Floor($allocations.Count / 2)]
    }
} | Format-Table -AutoSize

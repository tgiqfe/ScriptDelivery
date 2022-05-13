# テスト用コード

################################################################
# Prepare
################################################################

$url = "http://localhost:5000"



################################################################
# Main
################################################################

# / へのGetアクセス
Invoke-RestMethod -Method Get -Uri "${url}/"

# / へのPostアクセス可否
Invoke-RestMethod -Method Post -Uri "${url}/"

# /map へアクセスしてMapping情報を取得
$res = Invoke-RestMethod -Method Post -Uri "${url}/map"
Write-Host $res.Heaers
Write-Host "MappingList(JSON):`r`n" + $res.Body


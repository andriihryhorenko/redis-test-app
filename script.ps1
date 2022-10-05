$s = 1;

Do{
	Invoke-RestMethod -Uri 'http://localhost:9901/Api/probabilistic-read?key=1&ttl=1&beta=0.3'
	Start-Sleep -s 1
}
While($s -eq 1)
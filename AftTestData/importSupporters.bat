sqlcmd -Q "TRUNCATE TABLE Aft.dbo.Supporters"
bcp Aft.dbo.Supporters in "FiveThousandSupporters.dat" -n -f "FiveThousandSupporters.fmt" -S localhost -E -T -a 65535

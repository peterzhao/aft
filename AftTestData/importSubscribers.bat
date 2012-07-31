sqlcmd -Q "TRUNCATE TABLE Aft.dbo.Supporters"
bcp Aft.dbo.Supporters in "FiveThousandSubscribers.dat" -n -f "FiveThousandSubscribers.fmt" -S localhost -E -T -a 65535

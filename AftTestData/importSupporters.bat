sqlcmd -Q "TRUNCATE TABLE Aft.dbo.Supporters"
bcp Aft.dbo.Supporters in %1Supporters.dat -q -f %1Supporters.fmt -S localhost -T -a 65535

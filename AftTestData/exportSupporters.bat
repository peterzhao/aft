bcp Aft.dbo.Supporters format %1Supporters.dat -n -f %1Supporters.fmt -S localhost -T -a 65535
bcp Aft.dbo.Supporters    out %1Supporters.dat -f %1Supporters.fmt -S localhost -T -a 65535
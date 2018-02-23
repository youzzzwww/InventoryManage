create login [IIS APPPOOL\www_default_com] from windows;
exec sp_addsrvrolemember N'IIS APPPOOL\www_default_com', sysadmin
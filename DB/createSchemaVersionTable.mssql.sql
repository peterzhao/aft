IF NOT EXISTS (SELECT * FROM sysobjects WHERE Name= 'changelog' AND xType= 'U')
BEGIN

	CREATE TABLE changelog (
	  change_number INTEGER NOT NULL,
	  delta_set VARCHAR(10) NOT NULL,
	  start_dt DATETIME NOT NULL,
	  complete_dt DATETIME NULL,
	  applied_by VARCHAR(100) NOT NULL,
	  description VARCHAR(500) NOT NULL
	)

	ALTER TABLE changelog ADD CONSTRAINT Pkchangelog PRIMARY KEY (change_number, delta_set)
END

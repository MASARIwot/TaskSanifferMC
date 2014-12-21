CREATE TABLE IPAddreData
(
	Id INT NOT NULL PRIMARY KEY, 
    PortIn NCHAR(10) NULL, 
    PortOut NCHAR(10) NULL, 
    Protocol NCHAR(10) NULL, 
    UiSourceIPAddress NCHAR(25) NULL, 
    UiDestinationIPAddress NCHAR(25) NULL
)
CREATE TABLE `ipadapters_t` (
	`Id` INT(11) NOT NULL,
	`PortIn` VARCHAR(50) NULL DEFAULT NULL,
	`PortOut` VARCHAR(50) NULL DEFAULT NULL,
	`Protocol` VARCHAR(50) NULL DEFAULT NULL,
	`UiSourceIPAddress` VARCHAR(50) NULL DEFAULT NULL,
	`UiDestinationIPAddress` VARCHAR(50) NULL DEFAULT NULL,
	`MessageLength` VARCHAR(50) NULL DEFAULT NULL,
	PRIMARY KEY (`Id`)
)
COLLATE='utf8_general_ci'
ENGINE=InnoDB;
CREATE DATABASE  IF NOT EXISTS `t9-bd-2019630211` /*!40100 DEFAULT CHARACTER SET utf8mb3 */ /*!80016 DEFAULT ENCRYPTION='N' */;
USE `t9-bd-2019630211`;

DROP TABLE IF EXISTS `articulos`;

CREATE TABLE `articulos` (
  `idarticulos` int NOT NULL AUTO_INCREMENT,
  `nombre` varchar(45) NOT NULL,
  `descripcion` varchar(100) NOT NULL,
  `precio` float NOT NULL,
  `cantidad` int NOT NULL,
  `fotografia` longblob,
  PRIMARY KEY (`idarticulos`)
)
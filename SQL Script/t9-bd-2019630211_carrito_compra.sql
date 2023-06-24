DROP TABLE IF EXISTS `carrito_compra`;

CREATE TABLE `carrito_compra` (
  `cantidad` int NOT NULL,
  `idarticulos` int NOT NULL,
  KEY `idarticulos_idx` (`idarticulos`),
  CONSTRAINT `idarticulos` FOREIGN KEY (`idarticulos`) REFERENCES `articulos` (`idarticulos`) ON DELETE CASCADE ON UPDATE CASCADE
)
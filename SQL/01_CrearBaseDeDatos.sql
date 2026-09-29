-- =============================================
-- Parcial 1 - Laboratorio de Programación 3
-- Tienda Online: TechStore (tienda de tecnología)
-- Punto 1: Planificación del esquema relacional
-- =============================================

IF DB_ID('TiendaOnline') IS NULL
    CREATE DATABASE TiendaOnline;
GO

USE TiendaOnline;
GO

IF OBJECT_ID('dbo.productos', 'U') IS NOT NULL DROP TABLE dbo.productos;
IF OBJECT_ID('dbo.categorias', 'U') IS NOT NULL DROP TABLE dbo.categorias;
GO

-- Tabla 2: categorias
CREATE TABLE dbo.categorias (
    idCategoria  INT IDENTITY(1,1) NOT NULL,
    descripcion  VARCHAR(50)       NOT NULL,
    CONSTRAINT PK_categorias PRIMARY KEY (idCategoria),
    CONSTRAINT UQ_categorias_descripcion UNIQUE (descripcion)
);
GO

-- Tabla 1: productos
CREATE TABLE dbo.productos (
    idProducto   INT IDENTITY(1,1) NOT NULL,
    nombre       VARCHAR(100)      NOT NULL,
    precio       DECIMAL(10,2)     NOT NULL,
    idCategoria  INT               NOT NULL,
    CONSTRAINT PK_productos PRIMARY KEY (idProducto),
    CONSTRAINT CK_productos_precio CHECK (precio >= 0),
    -- Relación productos -> categorias.
    -- ON DELETE NO ACTION: no se puede borrar una categoría que tenga productos,
    -- y borrar un producto nunca afecta a la tabla categorias.
    CONSTRAINT FK_productos_categorias FOREIGN KEY (idCategoria)
        REFERENCES dbo.categorias (idCategoria)
        ON DELETE NO ACTION
        ON UPDATE NO ACTION
);
GO

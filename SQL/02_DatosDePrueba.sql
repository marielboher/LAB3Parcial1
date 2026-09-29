-- =============================================
-- Datos de prueba: categorías y 5 productos
-- (cubre "Configuración y Pruebas": al menos 5 productos cargados)
-- =============================================

USE TiendaOnline;
GO

INSERT INTO dbo.categorias (descripcion) VALUES
    ('Notebooks'),
    ('Celulares'),
    ('Periféricos'),
    ('Audio'),
    ('Monitores');
GO

INSERT INTO dbo.productos (nombre, precio, idCategoria) VALUES
    ('Notebook Lenovo IdeaPad 3',   850000.00, (SELECT idCategoria FROM dbo.categorias WHERE descripcion = 'Notebooks')),
    ('Samsung Galaxy A55',          620000.00, (SELECT idCategoria FROM dbo.categorias WHERE descripcion = 'Celulares')),
    ('Mouse Logitech G203',          35000.00, (SELECT idCategoria FROM dbo.categorias WHERE descripcion = 'Periféricos')),
    ('Auriculares JBL Tune 520BT',   75000.00, (SELECT idCategoria FROM dbo.categorias WHERE descripcion = 'Audio')),
    ('Monitor LG 24" IPS',          280000.00, (SELECT idCategoria FROM dbo.categorias WHERE descripcion = 'Monitores'));
GO

-- Verificación
SELECT p.idProducto, p.nombre, p.precio, c.descripcion AS categoria
FROM dbo.productos p
INNER JOIN dbo.categorias c ON c.idCategoria = p.idCategoria;
GO

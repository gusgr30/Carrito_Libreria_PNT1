# Carrito_Libreria_PNT1
Proyecto: E-Commerce de Libros
1. Objetivo de la aplicación
Desarrollar una aplicación web que permita a los usuarios explorar un catálogo de libros, agregarlos a un carrito de compras y realizar pedidos en línea. El sistema permitirá además la gestión del catálogo por parte del administrador (altas, bajas y modificaciones de libros) y la administración de usuarios registrados.

2. Funcionalidades de la aplicación

Módulo de Cliente:
- Registro e inicio de sesión de usuario.
- Navegación por catálogo de libros.
- Búsqueda por título, autor o categoría.
- Agregar libros al carrito.
- Ver y confirmar pedido.
- Consultar historial de compras.

Módulo de Administrador:
- Alta, baja y modificación de libros.
- Gestión de stock y precios.
- Administración de usuarios (opcional).
- Visualización de ventas realizadas.

3. Priorización de funcionalidades
<img width="900" height="258" alt="image" src="https://github.com/user-attachments/assets/a4b37456-9ab6-42cb-b156-4af047494260" />
 

4. Detalle de los pasos de cada funcionalidad

Alta de libro:
1. El administrador accede a la sección 'Libros'.
2. Presiona 'Agregar nuevo libro'.
3. Se muestra formulario con campos (título, autor, categoría, precio, stock, imagen).
4. Valida los campos obligatorios.
5. Se guarda en la base de datos.

Carrito de compras:
1. El usuario navega el catálogo y presiona 'Agregar al carrito'.
2. El sistema guarda los ítems en una sesión o tabla temporal.
3. El usuario puede ver su carrito y modificar cantidades.
4. Al confirmar, se genera un pedido asociado al usuario.

Login / Registro:
1. El usuario accede a 'Ingresar'.
2. Si no está registrado, completa formulario (nombre, mail, contraseña).
3. El sistema valida duplicados y almacena datos cifrados.
4. Si ya tiene cuenta, inicia sesión y accede al catálogo.

5. Datos y validaciones
<img width="900" height="400" alt="image" src="https://github.com/user-attachments/assets/78a309f5-9153-422f-b8f6-845db30a1528" />

 
6. Diagrama de clases de objetos de negocio
 <img width="900" height="725" alt="image" src="https://github.com/user-attachments/assets/44a90b42-1ec2-4299-ab5e-b3a6bcd40715" />


7. Esquema de páginas del sitio y navegación
<img width="289" height="270" alt="image" src="https://github.com/user-attachments/assets/747d653f-79d8-42ba-a3a4-932ec1d9eca6" />


Cada página contendrá los siguientes contenidos y acciones principales:
 
<img width="900" height="215" alt="image" src="https://github.com/user-attachments/assets/4da3fb24-4299-4206-9d7e-5ed9473b9bc4" />

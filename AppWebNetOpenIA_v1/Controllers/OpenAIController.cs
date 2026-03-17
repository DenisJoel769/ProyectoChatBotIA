using System.Diagnostics;
using System.Text;
using AppWebNetOpenIA_v1.Data;
using AppWebNetOpenIA_v1.Models;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.AI;

namespace AppWebNetOpenIA_v1.Controllers
{
    public class OpenAIController : Controller
    {
        private readonly IChatClient _chatClient;
        private static List<ChatMessage> _historial = new();
        private readonly ApplicationDbContext _context;
 


        public OpenAIController(IChatClient chatClient, ApplicationDbContext context)
        {
            _chatClient = chatClient;
            _context = context;

        }

        public IActionResult Index()
        {
            return View();
        }

       
        //Mostrar vista de generacion de textos
        public IActionResult GenerarTexto()
        {
           // var ventas_detalle = _context.VentasDetallesView.ToListAsync();
            return View();
        }
        //vista de ventas 
        public async Task<IActionResult> ConsultarVentas()
        {
            return View();
        }

        //vista de compras 
        public async Task<IActionResult> ConsultarCompras()
        {
            return View();
        }
        //consultar stock
        public async Task<IActionResult> ConsultarStock() 
        {
            return View();
        }
        //Mostrar vista para generacion de chat
        [HttpGet]
        public async Task<IActionResult> GenerarChats()
        {
            ViewBag.MostrarBoton = false; // o false según lógica
            return View();
        }
        //Metodo para consultar los datos de ventas 
        [HttpPost]
        public async Task<IActionResult> ConsultarDatosVentas(string consulta)
        {
            if (string.IsNullOrWhiteSpace(consulta))
                return RedirectToAction(nameof(ConsultarVentas));

            var ventas = await _context.VentasDetalles
                .Select(v => new
                {
                    v.ItemDescrip,
                    v.MarDescrip,
                    v.Precio,
                    v.Estado,
                    v.FechaVenta
                })
                .ToListAsync();

            if (!ventas.Any())
            {
                return View(nameof(ConsultarVentas),
                    "<div class='alert alert-warning'>No hay datos de ventas disponibles.</div>");
            }

            string consultaLower = consulta.ToLower();

            bool pideGrafico = consultaLower.Contains("grafico") ||
                               consultaLower.Contains("gráfico") ||
                               consultaLower.Contains("chart") ||
                               consultaLower.Contains("barra") ||
                               consultaLower.Contains("barras") ||
                               consultaLower.Contains("torta") ||
                               consultaLower.Contains("linea") ||
                               consultaLower.Contains("línea");

            bool pideConsejo = consultaLower.Contains("consejo") ||
                               consultaLower.Contains("recomendacion") ||
                               consultaLower.Contains("recomendación") ||
                               consultaLower.Contains("analisis") ||
                               consultaLower.Contains("análisis") ||
                               consultaLower.Contains("como vender") ||
                               consultaLower.Contains("cómo vender") ||
                               consultaLower.Contains("vender más") ||
                               consultaLower.Contains("mejorar ventas") ||
                               consultaLower.Contains("estrategia");

            var sb = new StringBuilder();

            sb.AppendLine("Eres un asistente especializado en análisis de ventas.");
            sb.AppendLine("Debes responder exclusivamente en HTML válido.");
            sb.AppendLine("No uses markdown.");
            sb.AppendLine("No escribas texto fuera del HTML.");
            sb.AppendLine("No inventes información que no esté presente en los datos.");
            sb.AppendLine("Si faltan datos para responder con exactitud, indícalo claramente.");
            sb.AppendLine();

            sb.AppendLine("REGLAS:");
            sb.AppendLine("- Si el usuario pide listar, mostrar, consultar, ver o comparar ventas, responde con una tabla HTML.");
            sb.AppendLine("- Si el usuario pide recomendaciones, consejos o análisis, responde con texto HTML claro y ordenado.");
            sb.AppendLine("- Si el usuario pide gráfico, responde con HTML y JavaScript usando Chart.js.");
            sb.AppendLine("- Si corresponde, puedes combinar una tabla con una breve conclusión.");
            sb.AppendLine("- Usa fondo blanco y texto negro.");
            sb.AppendLine("- En tablas con varios registros, usa cabecera negra con texto blanco.");
            sb.AppendLine("- Si el usuario pregunta qué producto se vende más, responde solo si puede inferirse de los datos.");
            sb.AppendLine("- Si no existen cantidades vendidas, aclara esa limitación.");
            sb.AppendLine();

            if (pideGrafico)
                sb.AppendLine("La intención principal del usuario es ver un gráfico.");
            else if (pideConsejo)
                sb.AppendLine("La intención principal del usuario es recibir recomendaciones o análisis.");
            else
                sb.AppendLine("La intención principal del usuario es consultar datos de ventas.");

            sb.AppendLine();
            sb.AppendLine($"CONSULTA DEL USUARIO: {consulta}");
            sb.AppendLine();
            sb.AppendLine("DATOS DE VENTAS:");

            foreach (var item in ventas)
            {
                string precio = Convert.ToDecimal(item.Precio).ToString("N0");

                sb.AppendLine(
                    $"Producto: {item.ItemDescrip} | " +
                    $"Marca: {item.MarDescrip} | " +
                    $"Precio: {precio} | " +
                    $"Estado: {item.Estado} | " +
                    $"FechaVenta: {item.FechaVenta:dd/MM/yyyy}"
                );
            }

            var response = await _chatClient.CompleteAsync(sb.ToString());

            return View(nameof(ConsultarVentas), response.ToString());
        }

        //metodo para consultar los datos de las compras 
        public async Task<IActionResult> ConsultarDatosCompras(string consulta)
        {
            if (string.IsNullOrWhiteSpace(consulta))
                return RedirectToAction(nameof(ConsultarCompras));

            var compraDetalle = await _context.ComprasDetalles
                .Select(v => new
                {
                    v.Item_Descripcion,
                    v.Cantidad,
                    v.Precio_Compra,
                    v.Precio_Venta,
                    v.Fecha
                })
                .ToListAsync();

            if (!compraDetalle.Any())
            {
                return View(nameof(ConsultarCompras),
                    "<div class='alert alert-warning'>No hay datos de compras disponibles.</div>");
            }

            string consultaLower = consulta.ToLower();

            bool pideGrafico = consultaLower.Contains("grafico") ||
                               consultaLower.Contains("gráfico") ||
                               consultaLower.Contains("chart") ||
                               consultaLower.Contains("barra") ||
                               consultaLower.Contains("barras") ||
                               consultaLower.Contains("torta") ||
                               consultaLower.Contains("linea") ||
                               consultaLower.Contains("línea");

            bool pideConsejo = consultaLower.Contains("consejo") ||
                               consultaLower.Contains("recomendacion") ||
                               consultaLower.Contains("recomendación") ||
                               consultaLower.Contains("analisis") ||
                               consultaLower.Contains("análisis") ||
                               consultaLower.Contains("como comprar") ||
                               consultaLower.Contains("cómo comprar") ||
                               consultaLower.Contains("comprar mejor") ||
                               consultaLower.Contains("optimizar compras");

            var sb = new StringBuilder();

            sb.AppendLine("Eres un asistente especializado en análisis de compras e inventario.");
            sb.AppendLine("Debes responder exclusivamente en HTML válido.");
            sb.AppendLine("No uses markdown.");
            sb.AppendLine("No escribas texto fuera del HTML.");
            sb.AppendLine("No inventes información no presente en los datos.");
            sb.AppendLine("Si faltan datos para responder con exactitud, indícalo claramente.");
            sb.AppendLine();

            sb.AppendLine("REGLAS:");
            sb.AppendLine("- Si el usuario pide listar, mostrar, consultar o comparar compras, responde con una tabla HTML.");
            sb.AppendLine("- Si el usuario pide recomendaciones o análisis, responde con texto HTML claro y ordenado.");
            sb.AppendLine("- Si el usuario pide gráfico, responde con HTML y JavaScript usando Chart.js.");
            sb.AppendLine("- Si corresponde, puedes combinar una tabla con una breve conclusión.");
            sb.AppendLine("- Usa fondo blanco y texto negro.");
            sb.AppendLine("- En tablas con varios registros, usa cabecera negra con texto blanco.");
            sb.AppendLine("- Si el usuario pregunta por tendencias o comportamiento de compras, responde solo con base en los datos disponibles.");
            sb.AppendLine("- Si el usuario pregunta algo que requiere datos no disponibles, acláralo.");
            sb.AppendLine();

            if (pideGrafico)
                sb.AppendLine("La intención principal del usuario es ver un gráfico.");
            else if (pideConsejo)
                sb.AppendLine("La intención principal del usuario es recibir recomendaciones o análisis.");
            else
                sb.AppendLine("La intención principal del usuario es consultar datos de compras.");

            sb.AppendLine();
            sb.AppendLine($"CONSULTA DEL USUARIO: {consulta}");
            sb.AppendLine();
            sb.AppendLine("DATOS DE COMPRAS:");

            foreach (var item in compraDetalle)
            {
                string precioCompra = Convert.ToDecimal(item.Precio_Compra).ToString("N0");
                string precioVenta = Convert.ToDecimal(item.Precio_Venta).ToString("N0");

                sb.AppendLine(
                    $"Producto: {item.Item_Descripcion} | " +
                    $"Cantidad: {item.Cantidad} | " +
                    $"PrecioCompra: {precioCompra} | " +
                    $"PrecioVenta: {precioVenta} | " +
                    $"Fecha: {item.Fecha:dd/MM/yyyy}"
                );
            }

            var response = await _chatClient.CompleteAsync(sb.ToString());

            return View(nameof(ConsultarCompras), response.ToString());
        }

        //metodo para consultar stock de productos 
        public async Task<IActionResult> ConsultarDatosStock(string consulta)
        {
            if (string.IsNullOrWhiteSpace(consulta))
                return RedirectToAction(nameof(ConsultarStock));

            var stockProductos = await _context.Stocks
                .Select(s => new
                {
                    s.NombreProducto,
                    s.Marca,
                    s.PrecioCompra,
                    s.PrecioVenta,
                    s.Cantidad
                })
                .ToListAsync();

            if (!stockProductos.Any())
            {
                return View(nameof(ConsultarStock), "<div class='alert alert-warning'>No hay datos de stock disponibles.</div>");
            }

            string consultaLower = consulta.ToLower();

            bool pideGrafico = consultaLower.Contains("grafico") ||
                               consultaLower.Contains("gráfico") ||
                               consultaLower.Contains("chart") ||
                               consultaLower.Contains("barra") ||
                               consultaLower.Contains("barras") ||
                               consultaLower.Contains("torta") ||
                               consultaLower.Contains("linea") ||
                               consultaLower.Contains("línea");

            bool pideConsejo = consultaLower.Contains("como vender") ||
                               consultaLower.Contains("cómo vender") ||
                               consultaLower.Contains("vender más") ||
                               consultaLower.Contains("recomendacion") ||
                               consultaLower.Contains("recomendación") ||
                               consultaLower.Contains("consejo");

            var sb = new StringBuilder();

            sb.AppendLine("Eres un asistente especializado en análisis de stock e inventario.");
            sb.AppendLine("Debes responder exclusivamente en HTML válido.");
            sb.AppendLine("No uses markdown.");
            sb.AppendLine("No escribas texto fuera del HTML.");
            sb.AppendLine("No inventes información no presente en los datos.");
            sb.AppendLine("Si faltan datos para responder algo con exactitud, indícalo claramente.");
            sb.AppendLine();

            sb.AppendLine("REGLAS:");
            sb.AppendLine("- Si el usuario pide productos o stock, usa tabla HTML.");
            sb.AppendLine("- Si el usuario pide consejos o recomendaciones, responde con texto HTML claro.");
            sb.AppendLine("- Si el usuario pide gráfico, responde con HTML + Chart.js.");
            sb.AppendLine("- Usa fondo blanco, texto negro y tablas legibles.");
            sb.AppendLine("- En tablas con varios registros, usa cabecera negra con texto blanco.");
            sb.AppendLine();

            if (pideGrafico)
                sb.AppendLine("La intención principal del usuario es ver un gráfico.");
            else if (pideConsejo)
                sb.AppendLine("La intención principal del usuario es recibir recomendaciones o análisis.");
            else
                sb.AppendLine("La intención principal del usuario es consultar datos del stock.");

            sb.AppendLine();
            sb.AppendLine($"CONSULTA DEL USUARIO: {consulta}");
            sb.AppendLine();
            sb.AppendLine("DATOS DISPONIBLES:");

            foreach (var item in stockProductos)
            {
                string precioCompra = Convert.ToDecimal(item.PrecioCompra).ToString("N0");
                string precioVenta = Convert.ToDecimal(item.PrecioVenta).ToString("N0");

                sb.AppendLine(
                    $"Producto: {item.NombreProducto} | " +
                    $"Marca: {item.Marca} | " +
                    $"Cantidad: {item.Cantidad} | " +
                    $"PrecioCompra: {precioCompra} | " +
                    $"PrecioVenta: {precioVenta}"
                );
            }

            var response = await _chatClient.CompleteAsync(sb.ToString());

            return View(nameof(ConsultarStock), response.ToString());
        }

        //FUNCIONALIDAD DE GENERACION DE CHATS CONVERSACIONAL 
        [HttpPost]
        public async Task<IActionResult> EnviarMensaje(string mensaje) 
        {
            if (string.IsNullOrEmpty(mensaje)) 
            {
                return View("GenerarChats");
            }
            _historial.Add(new ChatMessage(ChatRole.User, mensaje));

            //Generar respuestas de OpenAI usando chatclient
            var respuestaIA = await _chatClient.CompleteAsync(_historial);
            //verificar si a respuesta es valida
            string respuestaAsistente = respuestaIA.ToString() ?? "No se recibio respuestas de la IA";

            //agregar respues al historia
            _historial.Add(new ChatMessage(ChatRole.Assistant, respuestaAsistente));

            ViewBag.Historial = _historial ?? new List<ChatMessage>();

            //pasar la conversacion a la vista
            return View("GenerarChats");
        }

        public async Task<IActionResult> DescargarComprasExcel()
        {
            var stock = await _context.ComprasDetalles
                .Select(v => new
                {
                    v.Item_Descripcion,
                    v.Cantidad,
                    v.Precio_Compra,
                    v.Precio_Venta,
                    v.Fecha
                })
                .ToListAsync();

            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("ComprasDetalles");

            // Cabeceras
            worksheet.Cell(1, 1).Value = "Producto";
            worksheet.Cell(1, 2).Value = "Marca";
            worksheet.Cell(1, 3).Value = "Precio Compra";
            worksheet.Cell(1, 4).Value = "Precio Venta";
            worksheet.Cell(1, 5).Value = "Cantidad";

            // Estilo cabecera
            var headerRange = worksheet.Range(1, 1, 1, 5);
            headerRange.Style.Font.Bold = true;
            headerRange.Style.Fill.BackgroundColor = XLColor.Black;
            headerRange.Style.Font.FontColor = XLColor.White;

            int fila = 2;
            foreach (var item in stock)
            {
                worksheet.Cell(fila, 1).Value = item.Item_Descripcion;
                worksheet.Cell(fila, 2).Value = item.Cantidad;
                worksheet.Cell(fila, 3).Value = item.Precio_Compra;
                worksheet.Cell(fila, 4).Value = item.Precio_Venta;
                worksheet.Cell(fila, 5).Value = item.Fecha;
                fila++;
            }

            worksheet.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            var contenido = stream.ToArray();

            return File(
                contenido,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "StockProductos.xlsx"
            );
        }
        public async Task<IActionResult> DescargarVentasExcel()
        {
            var stock = await _context.VentasDetalles
                .Select(v => new
                {
                    v.ItemDescrip,
                    v.MarDescrip,
                    v.Precio,
                    v.Estado,
                    v.FechaVenta
                })
                .ToListAsync();

            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("VentasDetalles");

            // Cabeceras
            worksheet.Cell(1, 1).Value = "Producto";
            worksheet.Cell(1, 2).Value = "Marca";
            worksheet.Cell(1, 3).Value = "Precio Compra";
            worksheet.Cell(1, 4).Value = "Precio Venta";
            worksheet.Cell(1, 5).Value = "Cantidad";

            // Estilo cabecera
            var headerRange = worksheet.Range(1, 1, 1, 5);
            headerRange.Style.Font.Bold = true;
            headerRange.Style.Fill.BackgroundColor = XLColor.Black;
            headerRange.Style.Font.FontColor = XLColor.White;

            int fila = 2;
            foreach (var item in stock)
            {
                worksheet.Cell(fila, 1).Value = item.ItemDescrip;
                worksheet.Cell(fila, 2).Value = item.MarDescrip;
                worksheet.Cell(fila, 3).Value = item.Precio;
                worksheet.Cell(fila, 4).Value = item.Estado;
                worksheet.Cell(fila, 5).Value = item.FechaVenta;
                fila++;
            }

            worksheet.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            var contenido = stream.ToArray();

            return File(
                contenido,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "StockProductos.xlsx"
            );
        }
        public async Task<IActionResult> DescargarStockExcel()
        {
            var stock = await _context.Stocks
                .Select(s => new
                {
                    s.NombreProducto,
                    s.Marca,
                    s.PrecioCompra,
                    s.PrecioVenta,
                    s.Cantidad
                })
                .ToListAsync();

            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Stock");

            // Cabeceras
            worksheet.Cell(1, 1).Value = "Producto";
            worksheet.Cell(1, 2).Value = "Marca";
            worksheet.Cell(1, 3).Value = "Precio Compra";
            worksheet.Cell(1, 4).Value = "Precio Venta";
            worksheet.Cell(1, 5).Value = "Cantidad";

            // Estilo cabecera
            var headerRange = worksheet.Range(1, 1, 1, 5);
            headerRange.Style.Font.Bold = true;
            headerRange.Style.Fill.BackgroundColor = XLColor.Black;
            headerRange.Style.Font.FontColor = XLColor.White;

            int fila = 2;
            foreach (var item in stock)
            {
                worksheet.Cell(fila, 1).Value = item.NombreProducto;
                worksheet.Cell(fila, 2).Value = item.Marca;
                worksheet.Cell(fila, 3).Value = item.PrecioCompra;
                worksheet.Cell(fila, 4).Value = item.PrecioVenta;
                worksheet.Cell(fila, 5).Value = item.Cantidad;
                fila++;
            }

            worksheet.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            var contenido = stream.ToArray();

            return File(
                contenido,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "StockProductos.xlsx"
            );
        }
    }
}

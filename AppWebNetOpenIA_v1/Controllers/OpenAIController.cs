using System.Diagnostics;
using System.Text;
using AppWebNetOpenIA_v1.Data;
using AppWebNetOpenIA_v1.Models;
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
        //public async Task<IActionResult> ConsultarDatosVentas(string consulta)
        //{
        //    //construye un mensaje con la consulta del usuario 
        //    var prompt = $"el usuario pregunto: {consulta} \n\n aquí estan los datos de ventas: \n";
        //    var ventas_detalle = await _context.VentasDetalles.ToListAsync();
        //    //Agrega la informacion de cada transaccion al mensaje 
        //    foreach (var item in ventas_detalle)
        //    {
        //        prompt += $"Descripcion: {item.ItemDescrip}, Marca: {item.MarDescrip}" +
        //            $"Precio_venta: {item.Precio}" +
        //            $"Estado:{item.Estado}, Fecha_venta:{item.FechaVenta} \n";
        //    }

        //    //solicita a la IA que genere un resultado HTML + JavaScript
        //    prompt += "\n\n Por favor provea el resultado en formato HTML y JavaScript,por default muestra los resultados en una tabla y la cabecera en color negro, si te piden consejos, ocultar la tabla y mostrar solo los consejos en letras color negro y fondo blanco ";


        //    //Envia el mensaje al cliente de OpenAI para obtener una respuesta generada por IA 
        //    var response = await _chatClient.CompleteAsync(prompt);

        //    return View(nameof(ConsultarVentas), response);
        //}
        [HttpPost]
        public async Task<IActionResult> ConsultarDatosVentas(string consulta)
        {
            if (string.IsNullOrWhiteSpace(consulta))
                return RedirectToAction(nameof(ConsultarVentas));

            // 1. Proyectamos solo los datos necesarios para ahorrar memoria y ancho de banda
            var ventas = await _context.VentasDetalles
                .Select(v => new {
                    v.ItemDescrip,
                    v.MarDescrip,
                    v.Precio,
                    v.Estado,
                    v.FechaVenta
                })
                .ToListAsync();

            // 2. Usamos StringBuilder para una concatenación de strings eficiente
            var sb = new StringBuilder();
            sb.AppendLine($"El usuario preguntó: {consulta}");
            sb.AppendLine("Aquí están los datos de ventas:");

            foreach (var item in ventas)
            {
                sb.AppendLine($"- {item.ItemDescrip} ({item.MarDescrip}): {item.Precio:C} | Estado: {item.Estado} | Fecha: {item.FechaVenta:dd/MM/yyyy}");
            }

            // 3. Instrucciones claras de formato (System Prompt)
            sb.AppendLine("\nINSTRUCCIONES DE SALIDA:");
            sb.AppendLine("- Devuelve el resultado exclusivamente en formato HTML y JavaScript.");
            sb.AppendLine("- Por defecto, usa una tabla con cabecera negra.");
            sb.AppendLine("- Si el usuario pide consejos, oculta la tabla y muestra solo texto negro sobre fondo blanco.");

            // 4. Enviamos a la IA
            var response = await _chatClient.CompleteAsync(sb.ToString());

            // Pasamos el contenido de la respuesta (string) a la vista
            return View(nameof(ConsultarVentas), response.ToString());
        }

        //metodo para consultar los datos de las compras 
        public async Task<IActionResult> ConsultarDatosCompras(string consulta)
        {
            //construye un mensaje con la consulta del usuario 
            var prompt = $"el usuario pregunto: {consulta} \n\n aquí estan los datos de compras: \n";
            var ventas_detalle = await _context.ComprasDetalles.ToListAsync();
            //Agrega la informacion de cada transaccion al mensaje 
            foreach (var item in ventas_detalle)
            {
                prompt += $"Descripcion: {item.Item_Descripcion}, Cantidad: {item.Cantidad}" +
                    $"PrecioCompra: {item.Precio_Compra}" +
                    $"PrecioVenta:{item.Precio_Venta}, Fecha_Compra:{item.Fecha} \n";
            }

            //solicita a la IA que genere un resultado HTML + JavaScript
            prompt += "\n\n Por favor provea el resultado en formato HTML y JavaScript,no es necesario mostrar comentarios solo si pide el usuario,los resultados quiero que muestres por defaul en un tabla pero el usuario pide " +
                "que muestre en un grafico favor mostrar de esa forma";

            //Envia el mensaje al cliente de OpenAI para obtener una respuesta generada por IA 
            var response = await _chatClient.CompleteAsync(prompt);

            return View(nameof(ConsultarCompras), response);
        }

        //metodo para consultar stock de productos 
        public async Task<IActionResult> ConsultarDatosStock(string consulta) 
        {
            var prompt = $"El usuario pregunto : {consulta} \n\n aqui estan los productos disponibles: \n";
            var stock_productos = await _context.Stocks.ToListAsync();

            //agrega la informacion de cada transaccion al mensaje 
            foreach (var item in stock_productos) 
            {
                prompt += $"Descripcion: {item.NombreProducto},Marca: {item.Marca}" +
                    $"PrecioCompra: {item.PrecioCompra}, PrecioVenta: {item.PrecioVenta}" +
                    $"Cantidad: {item.Cantidad}";
            }
            prompt += "\n\n Por favor provea el resultado en formato HTML y JavaScript, solo muestra el resultado sin mostrar comentarios, muestra en una tabla, la cabecera en negro";
            return View(nameof(ConsultarDatosStock));

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
    }
}

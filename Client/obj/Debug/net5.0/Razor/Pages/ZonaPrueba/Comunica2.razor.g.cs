#pragma checksum "C:\Users\RogStrix_educas\source\repos\OverwatchBlazor\Client\Pages\ZonaPrueba\Comunica2.razor" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "4dc0dfd4236a90d5c0f9223ae09de34d82a74e79"
// <auto-generated/>
#pragma warning disable 1591
namespace OverwatchBlazor.Client.Pages.ZonaPrueba
{
    #line hidden
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Components;
#nullable restore
#line 1 "C:\Users\RogStrix_educas\source\repos\OverwatchBlazor\Client\_Imports.razor"
using System.Net.Http;

#line default
#line hidden
#nullable disable
#nullable restore
#line 2 "C:\Users\RogStrix_educas\source\repos\OverwatchBlazor\Client\_Imports.razor"
using System.Net.Http.Json;

#line default
#line hidden
#nullable disable
#nullable restore
#line 3 "C:\Users\RogStrix_educas\source\repos\OverwatchBlazor\Client\_Imports.razor"
using Microsoft.AspNetCore.Components.Forms;

#line default
#line hidden
#nullable disable
#nullable restore
#line 4 "C:\Users\RogStrix_educas\source\repos\OverwatchBlazor\Client\_Imports.razor"
using Microsoft.AspNetCore.Components.Routing;

#line default
#line hidden
#nullable disable
#nullable restore
#line 5 "C:\Users\RogStrix_educas\source\repos\OverwatchBlazor\Client\_Imports.razor"
using Microsoft.AspNetCore.Components.Web;

#line default
#line hidden
#nullable disable
#nullable restore
#line 6 "C:\Users\RogStrix_educas\source\repos\OverwatchBlazor\Client\_Imports.razor"
using Microsoft.AspNetCore.Components.Web.Virtualization;

#line default
#line hidden
#nullable disable
#nullable restore
#line 7 "C:\Users\RogStrix_educas\source\repos\OverwatchBlazor\Client\_Imports.razor"
using Microsoft.AspNetCore.Components.WebAssembly.Http;

#line default
#line hidden
#nullable disable
#nullable restore
#line 8 "C:\Users\RogStrix_educas\source\repos\OverwatchBlazor\Client\_Imports.razor"
using Microsoft.JSInterop;

#line default
#line hidden
#nullable disable
#nullable restore
#line 9 "C:\Users\RogStrix_educas\source\repos\OverwatchBlazor\Client\_Imports.razor"
using OverwatchBlazor.Client;

#line default
#line hidden
#nullable disable
#nullable restore
#line 10 "C:\Users\RogStrix_educas\source\repos\OverwatchBlazor\Client\_Imports.razor"
using OverwatchBlazor.Client.Shared;

#line default
#line hidden
#nullable disable
    [Microsoft.AspNetCore.Components.RouteAttribute("/Prueba/comunica2")]
    public partial class Comunica2 : Microsoft.AspNetCore.Components.ComponentBase
    {
        #pragma warning disable 1998
        protected override void BuildRenderTree(Microsoft.AspNetCore.Components.Rendering.RenderTreeBuilder __builder)
        {
            __builder.AddMarkupContent(0, "<h3>Comunica2, recibe el valor del hijo</h3>\r\n\r\n\r\n");
            __builder.OpenElement(1, "div");
            __builder.AddAttribute(2, "class", "row");
            __builder.OpenElement(3, "div");
            __builder.AddAttribute(4, "class", "col");
            __builder.OpenElement(5, "label");
            __builder.AddContent(6, 
#nullable restore
#line 9 "C:\Users\RogStrix_educas\source\repos\OverwatchBlazor\Client\Pages\ZonaPrueba\Comunica2.razor"
                textoRecibido

#line default
#line hidden
#nullable disable
            );
            __builder.CloseElement();
            __builder.CloseElement();
            __builder.AddMarkupContent(7, "\r\n    ");
            __builder.OpenElement(8, "div");
            __builder.AddAttribute(9, "class", "col");
            __builder.OpenComponent<OverwatchBlazor.Client.Pages.ZonaPrueba.Bebe2>(10);
            __builder.AddAttribute(11, "datoEnviar", Microsoft.AspNetCore.Components.CompilerServices.RuntimeHelpers.TypeCheck<Microsoft.AspNetCore.Components.EventCallback<System.String>>(Microsoft.AspNetCore.Components.EventCallback.Factory.Create<System.String>(this, 
#nullable restore
#line 12 "C:\Users\RogStrix_educas\source\repos\OverwatchBlazor\Client\Pages\ZonaPrueba\Comunica2.razor"
                           RecibeDato

#line default
#line hidden
#nullable disable
            )));
            __builder.CloseComponent();
            __builder.CloseElement();
            __builder.CloseElement();
        }
        #pragma warning restore 1998
#nullable restore
#line 20 "C:\Users\RogStrix_educas\source\repos\OverwatchBlazor\Client\Pages\ZonaPrueba\Comunica2.razor"
       

    public string textoRecibido;


    public void  RecibeDato(string dato)
    {
        this.textoRecibido = dato;
    }




#line default
#line hidden
#nullable disable
    }
}
#pragma warning restore 1591
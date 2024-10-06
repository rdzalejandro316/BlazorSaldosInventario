using System.Dynamic;
using Microsoft.AspNetCore.Components;
using AppSaldos.Data;
using Microsoft.JSInterop;

namespace AppSaldos.Components.Pages.Divided;

public partial class SaldosInventario : ComponentBase
{
    [Inject]
    public DataService DataService { get; set; } = default!;

    [Inject]
    public IJSRuntime JS { get; set; } = default!;

    private bool VisibleProperty { get; set; } = false;

    public List<ExpandoObject> SaldosInventarioList { get; set; } = new();
    public List<ExpandoObject> DataMarca { get; set; } = new();
    public List<ExpandoObject> DataLinea { get; set; } = new();
    public List<ExpandoObject> DataGrupo { get; set; } = new();
    public List<ExpandoObject> DataBodega { get; set; } = new();

    public DateTime DateValue { get; set; } = DateTime.Now;
    public string ValueMarca { get; set; } = "";
    public string ValueLinea { get; set; } = "";
    public string ValueGrupo { get; set; } = "";
    public string ValueBodega { get; set; } = "";
    public string ValueReferencia { get; set; } = "";

    protected override async Task OnInitializedAsync()
    {
        DataMarca = await DataService.GetDataAsync("select cod_mar,nom_mar from inmae_mar");
        DataLinea = await DataService.GetDataAsync("select cod_tip,nom_tip from inmae_tip");
        DataGrupo = await DataService.GetDataAsync("select cod_gru,nom_gru from inmae_gru");
        DataBodega = await DataService.GetDataAsync("select cod_bod,nom_bod from inmae_bod");
    }

    public async Task Consultar()
    {
        try
        {
            if (string.IsNullOrEmpty(ValueBodega))
            {
                await JS.InvokeVoidAsync("alert", "Debe de seleccionar una bodega");
                return;
            }

            string fecha = DateValue.ToString("dd/MM/yyyy");

            var parameters = new Dictionary<string, object>
                {
                    { "@Fecha", fecha },
                    { "@Bod", ValueBodega },
                    { "@Mar", ValueMarca },
                    { "@Tip", ValueLinea },
                    { "@Gru", ValueGrupo },
                    { "@tipo", "1" },
                    { "@codemp", "010" },
                    { "@referencia", ValueReferencia }
                };


            await JS.InvokeVoidAsync("console.log", "aaaa", parameters);


            VisibleProperty = true;
            var data = await DataService.ExecuteStoredProcedureAsync("_EmpSaldosInventarios", parameters);
            SaldosInventarioList = data;
            VisibleProperty = false;

            //StateHasChanged();
        }
        catch (Exception ex)
        {
            await JS.InvokeVoidAsync("console.error", $"Error: {ex.Message}");
        }
    }

    public void Limpiar()
    {
        DateValue = DateTime.Now;
        ValueMarca = "";
        ValueLinea = "";
        ValueGrupo = "";
        ValueBodega = "";
        ValueReferencia = "";
    }
}


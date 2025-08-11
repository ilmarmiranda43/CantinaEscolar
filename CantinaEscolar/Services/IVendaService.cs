using System.Threading.Tasks;
using CantinaEscolar.ViewModels;

namespace CantinaEscolar.Services
{
    public interface IVendaService
    {
        Task<VendaViewModel> CarregarTelaAsync(int? alunoId = null);
        Task<(bool ok, string mensagem)> EfetivarVendaAsync(VendaViewModel vm);
        Task<decimal> ObterLimiteDisponivelAsync(int alunoId);
    }
}

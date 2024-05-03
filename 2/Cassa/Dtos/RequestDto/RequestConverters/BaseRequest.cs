using lab_1.Cassa.Entities;
using Mapster;

namespace lab_1.Cassa.Dtos.RequestDto.RequestConverters
{

    public class BaseRequest<T, TV> where T : TblBase
    {
        public T FromDto(TV dto)
        {
            
            return dto.Adapt<T>();
        }
    }
}

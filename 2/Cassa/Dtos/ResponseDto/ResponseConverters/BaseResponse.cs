using lab_1.Cassa.Entities;
using Mapster;

namespace lab_1.Cassa.Dtos.ResponseDto.ResponseConverters
{
    public class BaseResponse<T,V> where V:TblBase
    {
        public T ToDto(V entity) => entity.Adapt<T>();
    }
}

namespace EIP.Common.Models.Dtos
{
    /// <summary>
    /// 此接口可以用来标记DTO既可以作为输出DTO也可以作为输入DTO
    /// </summary>
    public interface IDoubleWayDto : IInputDto, IOutputDto
    {

    }
}
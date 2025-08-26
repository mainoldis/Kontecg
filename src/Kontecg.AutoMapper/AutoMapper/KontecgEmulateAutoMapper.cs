using System;
using AutoMapper;

namespace Kontecg.AutoMapper
{
    public static class KontecgEmulateAutoMapper
    {
        [Obsolete("Automapper will remove static API, Please use ObjectMapper instead. See https://github.com/aspnetboilerplate/aspnetboilerplate/issues/4667")]
        public static IMapper Mapper { get; set; }
    }
}

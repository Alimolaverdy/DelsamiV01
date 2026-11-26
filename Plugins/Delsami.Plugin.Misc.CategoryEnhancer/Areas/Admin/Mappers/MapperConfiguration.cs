using AutoMapper;
using Delsami.Plugin.Misc.CategoryEnhancer.Areas.Admin.Models;
using Nop.Core.Infrastructure.Mapper;
using Nop.Web.Areas.Admin.Models.Catalog;

namespace Delsami.Plugin.Misc.CategoryEnhancer.Areas.Admin.Mappers
{
    internal class MapperConfiguration : Profile, IOrderedMapperProfile
    {
        public MapperConfiguration()
        {
            CreateCategoryMap();
        }

        public int Order => 1;

        private void CreateCategoryMap()
        {
            CreateMap<CategoryModel, DelsamiCategoryModel>();
        }
    }
}

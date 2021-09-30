
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using projektas.Context;
using System.Data.Entity.Core.Objects;

using System.Data.Entity.Core.Metadata.Edm;

namespace projektas.Controllers
{

    public class DBController
    {
        public T Save<T>(T model) where T : class, BaseModel
        {
            using (var context = new MovieDbContext())
            {
                Type type = model.GetType();
                context.Set<T>().Attach(model);
                context.Set<T>().Add(model);
                context.SaveChanges();
                return model;
            }
        }
        public T Update<T>(T model) where T : class, BaseModel
        {
            using (var context = new MovieDbContext())
            {
                Type type = model.GetType();
                context.Entry(model).State = EntityState.Modified;
                context.SaveChanges();
                return model;
            }
        }
        public T Delete<T>(T model) where T : class, BaseModel
        {
            using (var context = new MovieDbContext())
            {
                Type type = model.GetType();
                context.Entry(model).State = EntityState.Deleted;
                context.SaveChanges();
                return model;
            }
        }

        public T Get<T>(Expression<Func<T, bool>> predicate, bool include = false)
    where T : class, BaseModel
        {
            T item = null;
            using (var context = new MovieDbContext())
            {
                var set = context.Set<T>();
                if (include)
                {
                    var tables = GetTables(context);
                    var properties = typeof(T).GetProperties();

                    var tableNames = tables.Select(x => x.Name).Intersect(properties.Select(y => y.Name));
                    DbSet<T> query = null;
                    foreach (var table in tableNames)
                    {
                        query = (DbSet<T>)set.Include(table);
                    }
                    item = query?.FirstOrDefault(predicate);
                }
                else
                {
                    item = set.FirstOrDefault(predicate);
                }
                return item;
            }
        }



        public IList<T> GetList<T>(Expression<Func<T, bool>> predicate, bool include = false)
    where T : class, BaseModel
        {
            IList<T> item = null;
            using (var context = new MovieDbContext())
            {
                var set = context.Set<T>();
                if (include)
                {
                    var tables = GetTables(context);
                    var properties = typeof(T).GetProperties();

                    var tableNames = tables.Select(x => x.Name).Intersect(properties.Select(y => y.Name));
                    DbSet<T> query = null;
                    foreach (var table in tableNames)
                    {
                        query = (DbSet<T>)set.Include(table);
                    }
                    item = query?.Where(predicate).ToList();
                }
                else
                {
                    item = set.Where(predicate).ToList();
                }
                return item;
            }
        }

        public IList<T> GetAll<T>(bool include = false)
    where T : class, BaseModel
        {
            IList<T> item = null;
            using (var context = new MovieDbContext())
            {
                item = context.Set<T>().ToList();
            }
            return item;
        }

        private PropertyInfo[] GetPropertiesInfo<T>(T model)
        {
            PropertyInfo[] propertyInfos;
            propertyInfos = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Static);
            return propertyInfos;
        }

        private IEnumerable<EntityType> GetTables(MovieDbContext context)
        {
            ObjectContext objContext = ((System.Data.Entity.Infrastructure.IObjectContextAdapter)context).ObjectContext;
            MetadataWorkspace workspace = objContext.MetadataWorkspace;
            IEnumerable<EntityType> tables = workspace.GetItems<EntityType>(DataSpace.SSpace);
            return tables;
        }
    }
}
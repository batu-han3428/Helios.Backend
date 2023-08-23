namespace Helios.Authentication.Helpers
{
    public class Mapper
    {
        public static TTarget Map<TSource, TTarget>(TSource ts, TTarget tt)
        {
            foreach (var item in ts.GetType().GetProperties())
            {
                string propName = item.Name;
                if (propName == "Id")
                    continue;
                object propVal = item.GetValue(ts);

                foreach (var subItem in tt.GetType().GetProperties())
                {
                    if (subItem.Name == propName)
                    {
                        subItem.SetValue(tt, propVal);
                    }
                }
            }

            return tt;
        }

        public static List<T> MapList<T, TE>(List<TE> oList) where T : class where TE : class
        {
            List<T> vm = Activator.CreateInstance<List<T>>();
            if (oList != null)
            {
                foreach (object o in oList)
                {
                    T genericObject = Activator.CreateInstance<T>();
                    foreach (var item in o.GetType().GetProperties())
                    {
                        string propName = item.Name;
                        object propVal = item.GetValue(o);

                        foreach (var genericObjectItem in genericObject.GetType().GetProperties())
                        {
                            if (genericObjectItem.Name == propName)
                            {
                                genericObjectItem.SetValue(genericObject, propVal);
                            }
                        }
                    }
                    vm.Add(genericObject);
                }
            }

            return vm;
        }

    }
}

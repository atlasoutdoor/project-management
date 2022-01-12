/*
Copyright © 2013 Annpoint, s.r.o.

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.

-------------------------------------------------------------------------

NOTE: Reuse requires the following acknowledgement (see also NOTICE):
This product includes DayPilot (http://www.daypilot.org) developed by Annpoint, s.r.o.
*/

using System;
using System.Data;
using System.Reflection;
using System.Web.UI;
using PropertyAttributes = System.Data.PropertyAttributes;

namespace Util.Ui
{
    public class Binder
    {
        public static DataItem Get(object o, string property)
        {
            if (o is DataRow)
            {
                DataRow dr = (DataRow) o;
                return new DataItem() { Source = dr[property]};
            }
            return new DataItem() { Source = ReadPropertyValue(o, property) };

        }

        private static object ReadPropertyValue(object o, string property)
        {
            Type type = o.GetType();
            PropertyInfo pi = type.GetProperty(property);
            if (pi != null)
            {
                return pi.GetValue(o, null);
            }
            else // try to read using indexed property
            {
                MethodInfo mi = type.GetMethod("get_Item", new Type[] {typeof (String)});
                if (mi != null)
                {
                    return mi.Invoke(o, new object[] {property});
                }
            }

            throw new ArgumentException("Property or index not found.");
        }

        public static DataItem Get(object value)
        {
            return new DataItem() { Source = value };
        }

        public class DataItem
        {
            internal object Source;

            public object Object
            {
                get
                {
                    if (Source == DBNull.Value)
                    {
                        return null;
                    }
                    return Source;
                }
            }
            public string String
            {
                get
                {
                    if (Source == DBNull.Value || Source == null)
                    {
                        return null;
                    }
                    return Convert.ToString(Source);
                }
            }

            public DateTime? DateTime
            {
                get
                {
                    if (Source == DBNull.Value || Source == null)
                    {
                        return null;
                    }
                    if (Source is string && (string)Source == String.Empty)
                    {
                        return null;
                    }
                    return Convert.ToDateTime(Source);
                }
            }

            public int? Int32
            {
                get
                {
                    if (Source == DBNull.Value || Source == null)
                    {
                        return null;
                    }
                    if (Source is string && (string) Source == String.Empty)
                    {
                        return null;
                    }
                    return Convert.ToInt32(Source);
                }
            }

            public double? Double
            {
                get
                {
                    if (Source == DBNull.Value || Source == null)
                    {
                        return null;
                    }
                    if (Source is string && (string) Source == String.Empty)
                    {
                        return null;
                    }
                    return Convert.ToDouble(Source);
                }
            }

            public bool IsNull
            {
                get { return Source == null || Source == DBNull.Value; }
            }
            
            public bool IsNotNull
            {
                get { return !IsNull; }
            }

            public TimeSpan TimeSpanFromMinutes
            {
                get
                {
                    if (IsNull)
                    {
                        return TimeSpan.Zero;
                    }
                    return TimeSpan.FromMinutes((double) Double);
                }
            }
        }

    }

    
}
using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Reflection;
using System.Data.SqlClient;
using System.Data;

namespace MyNameSpace
{
	public class AttributeUtilities
	{
		SqlParameterCollection parmCol;

        public SqlParameterCollection ConvertSQLParameterArray(SqlParameter[] parameters)
        {
            SqlParameterCollection spCol = new SqlCommand().Parameters;
            spCol.AddRange(parameters);
            return spCol;
        }

        public SqlParameter[] ConvertSQLParmeterCollection(SqlParameterCollection spc)
        {
            int iCounter = 0;
            SqlParameter[] sp = new SqlParameter[spc.Count];

            foreach (SqlParameter spTmp in spc)
            {
                sp[iCounter] = new SqlParameter();
                sp[iCounter].DbType = spTmp.DbType;
                sp[iCounter].Direction = spTmp.Direction;
                sp[iCounter].ParameterName = spTmp.ParameterName;
                sp[iCounter].Size = spTmp.Size;
                sp[iCounter].SqlDbType = spTmp.SqlDbType;
                sp[iCounter].SqlValue = spTmp.SqlValue;
                sp[iCounter].Value = spTmp.Value;
                iCounter++;
            }

            return sp;

        }

        public SqlParameterCollection CreateParametersColUsingAttributes(Type ClsObjectType, object ClassObject)
        {
            parmCol = new SqlCommand().Parameters;

            foreach (PropertyInfo property in ClsObjectType.GetProperties())
            {
                SetSqlParameterValues(property, ClassObject);
            }

            return parmCol;
        }

        public SqlParameter[] CreateParametersUsingAttributes(Type ClsObjectType, object ClassObject)
		{
			BIRP.PIRP.PIRPData oData = new PIRPData();
			return ConvertSQLParmeterCollection(this.CreateParametersColUsingAttributes(ClsObjectType, ClassObject));
		}

		private void SetSqlParameterValues(PropertyInfo property, object ClassObject)
		{
			SqlParameter parameter = new SqlParameter();

			foreach (object attribute in property.GetCustomAttributes(true))
			{
				if (attribute is CustomAttributes.SqlColumnAttribute)
				{
					CustomAttributes.SqlColumnAttribute SqlColumn = (CustomAttributes.SqlColumnAttribute)attribute;

					if (SqlColumn.IsNameDefined)
					{
						parameter.ParameterName = SqlColumn.Name; 
					}

					if (SqlColumn.IsTypeDefined)
					{
						parameter.SqlDbType = SqlColumn.SqlDbType; 
					}

                    if (SqlColumn.IsDirectionDefined)
                    {
                        parameter.Direction = SqlColumn.Direction;
                    }

					if (SqlColumn.IsSizeDefined)
					{
						parameter.Size = SqlColumn.Size; 
					}

                    try
                    {
                        if (property.CanRead)
                        {
                            if (IsNullableType(property.PropertyType) &&
                                property.GetValue(ClassObject, null) == null)
                            {
                                parameter.Value = DBNull.Value;
                            }
                            else
                            {
                                //TODO:  Earl/Keith - come up with a way to test the ability to cast the value 
                                //       to SqlDbType
                                //parameter.Value = (parameter.SqlDbType.GetType()) property.GetValue(ClassObject, null);
                                parameter.Value = property.GetValue(ClassObject, null);
                            }
                        }
                    }
                    catch (Exception exp)
                    {
                        exp.Data.Add("Class Object", ClassObject.ToString());
						exp.Data.Add("Property Name", property.Name);
						exp.Data.Add("Property Type", property.PropertyType.ToString());
                        if (parameter != null && parameter.Value != null)
                        {
							exp.Data.Add("Property Value", parameter.Value.ToString());
                        }
                        throw;
                    }
					
					parmCol.Add(parameter);

				}
			}
		}

        private bool IsNullableType(Type myType)
        {
            return (myType.IsGenericType && myType.GetGenericTypeDefinition().Equals(typeof(Nullable<>)));
        }

		public void SetPropertiesUsingSqlColumnAttributes(Type ClsObjectType, object ClassObject, SqlDataReader DataReader)
		{
			foreach (PropertyInfo property in ClsObjectType.GetProperties())
			{
				SetPropertyValuesBySqlColumnAttribute(property, ClassObject, DataReader);
			}
		}

		public void SetPropertiesUsingSqlColumnAttributes(Type ClsObjectType, object ClassObject, DataRow Row)
		{
			foreach (PropertyInfo property in ClsObjectType.GetProperties())
			{
				SetPropertyValuesBySqlColumnAttribute(property, ClassObject, Row);
			}
		}

		private void SetPropertyValuesBySqlColumnAttribute(PropertyInfo property, object ClassObject, SqlDataReader DataReader)
		{
			SqlParameter parameter = new SqlParameter();

			foreach (object attribute in property.GetCustomAttributes(true))
			{
				if (attribute is CustomAttributes.SqlColumnAttribute)
				{
					CustomAttributes.SqlColumnAttribute SqlColumn = (CustomAttributes.SqlColumnAttribute)attribute;

					if (SqlColumn.IsNameDefined)
					{
						if (property.CanWrite)
						{
							if (DataReader[SqlColumn.Name] != null &&
							    DataReader[SqlColumn.Name].ToString().Length > 0)
							{
								try
								{
									if (property.PropertyType == typeof(System.Double))
									{
										double dbl = Convert.ToDouble(DataReader[SqlColumn.Name]);
										property.SetValue(ClassObject, dbl, null);
									}
									else
									{
										property.SetValue(ClassObject, DataReader[SqlColumn.Name], null);
									}
								}
								catch (Exception exp)
								{
									exp.Data.Add("Class Object", ClassObject.ToString());
									exp.Data.Add("Property Name", property.Name);
									exp.Data.Add("Property Type", property.PropertyType.ToString());
									exp.Data.Add("Property Readable", property.CanRead.ToString());
									exp.Data.Add("Property Writeable", property.CanWrite.ToString());
									exp.Data.Add("Column Name", SqlColumn.Name);
									exp.Data.Add("Column Value", DataReader[SqlColumn.Name].ToString());
									exp.Data.Add("Column Type", DataReader[SqlColumn.Name].GetType().ToString());
									throw;
								}
							}
						}
					}
				}
			}
		}

		private void SetPropertyValuesBySqlColumnAttribute(PropertyInfo property, object ClassObject, DataRow Row)
		{
			SqlParameter parameter = new SqlParameter();

			foreach (object attribute in property.GetCustomAttributes(true))
			{
				if (attribute is CustomAttributes.SqlColumnAttribute)
				{
					CustomAttributes.SqlColumnAttribute SqlColumn = (CustomAttributes.SqlColumnAttribute)attribute;

					if (SqlColumn.IsNameDefined)
					{
						if (property.CanWrite)
						{
							if (Row[SqlColumn.Name] != null &&
								Row[SqlColumn.Name].ToString().Length > 0)
							{
								try
								{
                                  	if (property.PropertyType == typeof(System.Double))
									{
										double dbl = Convert.ToDouble(Row[SqlColumn.Name]);
										property.SetValue(ClassObject, dbl, null);
									}
                                    else if (property.PropertyType == typeof(System.Decimal))
                                    {
                                        decimal dec = Convert.ToDecimal(Row[SqlColumn.Name]);
                                        property.SetValue(ClassObject, dec, null);
                                    }
									else
									{
										property.SetValue(ClassObject, Row[SqlColumn.Name], null);
									}
								}
								catch (Exception exp)
								{
									exp.Data.Add("Class Object", ClassObject.ToString());
									exp.Data.Add("Property Name", property.Name);
									exp.Data.Add("Property Type", property.PropertyType.ToString());
									exp.Data.Add("Property Readable", property.CanRead.ToString());
									exp.Data.Add("Property Writeable", property.CanWrite.ToString());
									exp.Data.Add("Column Name", SqlColumn.Name);
									exp.Data.Add("Column Value", Row[SqlColumn.Name].ToString());
									exp.Data.Add("Column Type", Row[SqlColumn.Name].GetType().ToString());
									throw;
								}
							}
						}
					}
				}
			}
		}
	}
}

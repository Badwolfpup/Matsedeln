//using Matsedeln.Models;
//using Microsoft.Data.SqlClient;
//using Microsoft.Extensions.Configuration;
//using System;
//using System.Collections.Generic;
//using System.Collections.ObjectModel;
//using System.Configuration;
//using System.Diagnostics;
//using System.Printing;
//using System.Runtime.InteropServices;
//using System.Text;
//using System.Threading.Tasks;
//using System.Windows.Documents.DocumentStructures;

//namespace Matsedeln.Utils
//{
//    public static class DBHelper
//    {
//        private static readonly string connectionString = Matsedeln.App.Configuration.GetConnectionString("VeckoMenyDB");

//        public static async Task<SqlConnection> ConnectToDatabase(string ConnectionString)
//        {
//            var connect = new SqlConnection(ConnectionString);
//            if (connect == null)
//            {
//                Debug.WriteLine("Error: Unable to create a database connection.");
//                return null;
//            }
//            try
//            {
//                await connect.OpenAsync();
//                Debug.WriteLine("Database connection established successfully.");
//                return connect;
//            }
//            catch (SqlException ex)
//            {
//                // Handle the exception
//                Debug.WriteLine("Error: " + ex.Message);
//                connect?.Dispose();
//                connect = null;
//                return connect;
//            }
//            catch (Exception ex)
//            {
//                // Catch other general errors (e.g., network issues)
//                Debug.WriteLine($"Error: {ex.Message}");
//                connect?.Dispose();
//                connect = null;
//                return connect;
//            }

//        }

//        public static async Task<bool> AddGoodsToDB(Goods good)
//        {
//            var connect = await ConnectToDatabase(connectionString);
//            if (connect == null)
//            {
//                return false; 
//            }
//            try
//            {
//                string insertQuery = "INSERT INTO Goods (Name, ImagePath, GperDL, GperST) VALUES (@Name, @ImagePath, @GperDL, @GperST)";
//                using (SqlCommand command = new SqlCommand(insertQuery, connect))
//                {
//                    command.Parameters.AddWithValue("@Name", good.Name);
//                    command.Parameters.AddWithValue("@ImagePath", good.ImagePath);
//                    command.Parameters.AddWithValue("@GperDL", good.GramsPerDeciliter);
//                    command.Parameters.AddWithValue("@GperST", good.GramsPerStick);
//                    int rowsAffected = await command.ExecuteNonQueryAsync();
//                    Debug.WriteLine($"{rowsAffected} row(s) inserted.");
//                }
//                return true;
//            }
//            catch (SqlException ex)
//            {
//                Debug.WriteLine("SQL Error: " + ex.Message);
//                return false;
//            }
//            catch (Exception ex)
//            {
//                Debug.WriteLine("Error: " + ex.Message);
//                return false;
//            }
//            finally
//            {
//                await connect.CloseAsync();
//            }
//        }

//        public static async Task<ObservableCollection<Goods>> GetAllGoodsFromDB()
//        {
//            var connect = await ConnectToDatabase(connectionString);
//            if (connect == null) return new ObservableCollection<Goods>();

//            try
//            {
//                string sqlquery = "SELECT Id, Name, ImagePath, GperDL, GperST FROM Goods";
//                using (var command = connect.CreateCommand())
//                {
//                    command.CommandText = sqlquery;
//                    using (var reader = await command.ExecuteReaderAsync())
//                    {
//                        var goods = new ObservableCollection<Goods>();
//                        while (await reader.ReadAsync())
//                        {
//                            goods.Add(new Goods
//                            {
//                                Id = reader.GetInt32(0),
//                                Name = reader.GetString(1),
//                                ImagePath = reader.GetString(2),
//                                GramsPerDeciliter = reader.GetInt32(3),
//                                GramsPerStick = reader.GetInt32(4)
//                            });
//                        }
//                        return goods;
//                    }
//                }
//            }
//            catch (SqlException ex)
//            {
//                Debug.WriteLine("SQL Error: " + ex.Message);
//                return new ObservableCollection<Goods>();
//            }
//            catch (Exception ex)
//            {
//                Debug.WriteLine("Error: " + ex.Message);
//                return new ObservableCollection<Goods>();
//            }
//            finally
//            {
//                await connect.CloseAsync();
//            }
//        }

//        public static async Task<bool> UpdateGoodsInDB(Goods nyVara)
//        {
//            var connect = await ConnectToDatabase(connectionString);
//            if (connect == null) return false;

//            try
//            {
//                string sqlquery = "UPDATE Goods SET Name = @Name, ImagePath = @Imagepath, GperDL = @GperDL, GperST = @GperST WHERE Id = @Id";
//                using (var command = connect.CreateCommand())
//                {
//                    command.CommandText = sqlquery;
//                    command.Parameters.AddWithValue("@Name", nyVara.Name);
//                    command.Parameters.AddWithValue("@Imagepath", nyVara.ImagePath);
//                    command.Parameters.AddWithValue("@GperDL", nyVara.GramsPerDeciliter);
//                    command.Parameters.AddWithValue("@GperST", nyVara.GramsPerStick);
//                    command.Parameters.AddWithValue("@Id", nyVara.Id);
//                    int rows = await command.ExecuteNonQueryAsync();
//                    return rows > 0;

//                }
//            }
//            catch (SqlException ex)
//            {
//                Debug.WriteLine("SQL Error: " + ex.Message);
//                return false;
//            }
//            catch (Exception ex)
//            {
//                Debug.WriteLine("Error: " + ex.Message);
//                return false;
//            }
//            finally
//            {
//                await connect.CloseAsync();
//            }
//        }

//        public static async Task<bool> AddRecipeToDB(Recipe recipe)
//        {
//            var connect = await ConnectToDatabase(connectionString);
//            int recipeId = -1;
//            if (connect == null) return false;
//            try
//            {
//                string sqlquery = "INSERT INTO Recipe (Name, Imagepath) VALUES (@Name, @Imagepath); SELECT SCOPE_IDENTITY();";
//                using (var command = connect.CreateCommand())
//                {
//                    command.CommandText = sqlquery;
//                    command.Parameters.AddWithValue("@Name", recipe.Name);
//                    command.Parameters.AddWithValue("@Imagepath", recipe.ImagePath ?? (object)DBNull.Value);
//                    var id = await command.ExecuteScalarAsync();
//                    recipeId = Convert.ToInt32(id);
//                    foreach (var ingredient in recipe.Ingredientlist)
//                    {
//                        int goodsId = await GetGoodsId(ingredient.Good);
//                        if (goodsId == -1)
//                        {
//                            Debug.WriteLine("Error: Goods not found in database.");
//                            continue;
//                        }
//                        string insertIngredientQuery = "INSERT INTO Ingredient (Recipe_Id, Goods_Id, Quantity, Unit) VALUES (@RecipeId, @GoodsId, @Quantity, @Unit)";
//                        using (var ingredientCommand = connect.CreateCommand())
//                        {
//                            ingredientCommand.CommandText = insertIngredientQuery;
//                            ingredientCommand.Parameters.AddWithValue("@RecipeId", recipeId);
//                            ingredientCommand.Parameters.AddWithValue("@GoodsId", goodsId);
//                            ingredientCommand.Parameters.AddWithValue("@Quantity", ingredient.Quantity);
//                            ingredientCommand.Parameters.AddWithValue("@Unit", ingredient.Unit);
//                            await ingredientCommand.ExecuteNonQueryAsync();
//                        }
//                    }
//                    return true;
//                }
//            }
//            catch (SqlException ex)
//            {
//                if (recipeId != -1)
//                {
//                    string sqlquery = "DELETE FROM Recipe WHERE Id = @Id";
//                    using (var command = connect.CreateCommand())
//                    {
//                        command.CommandText = sqlquery;
//                        command.Parameters.AddWithValue("@Id", recipeId);
//                        await command.ExecuteNonQueryAsync();
//                    }
//                }
//                Debug.WriteLine("SQL Error: " + ex.Message);
//                return false;
//            }
//            catch (Exception ex)
//            {
//                if (recipeId != -1)
//                {
//                    string sqlquery = "DELETE FROM Recipe WHERE Id = @Id";
//                    using (var command = connect.CreateCommand())
//                    {
//                        command.CommandText = sqlquery;
//                        command.Parameters.AddWithValue("@Id", recipeId);
//                        await command.ExecuteNonQueryAsync();
//                    }
//                }
//                Debug.WriteLine("Error: " + ex.Message);
//                return false;
//            }
//            finally
//            {
//                await connect.CloseAsync();
//            }
//        }

//        private static async Task<int> GetGoodsId(Goods good)
//        {
//            var connect = await ConnectToDatabase(connectionString);
//            if (connect == null) return -1;
//            try
//            {
//                string sqlquery = "SELECT Id FROM Goods WHERE Name = @Name";
//                using (var command = connect.CreateCommand())
//                {
//                    command.CommandText = sqlquery;
//                    command.Parameters.AddWithValue("@Name", good.Name);
//                    var result = await command.ExecuteScalarAsync();
//                    if (result != null && int.TryParse(result.ToString(), out int id))
//                    {
//                        return id;
//                    }
//                    else
//                    {
//                        return -1;
//                    }
//                }
//            }
//            catch (SqlException ex)
//            {
//                Debug.WriteLine("SQL Error: " + ex.Message);
//                return -1;
//            }
//            catch (Exception ex)
//            {
//                Debug.WriteLine("Error: " + ex.Message);
//                return -1;
//            }
//            finally
//            {
//                await connect.CloseAsync();
//            }
//        }

//        public static async Task<ObservableCollection<Recipe>> GetAllRecipesFromDB(ObservableCollection<Goods> goods)
//        {
//            var connect = await ConnectToDatabase(connectionString);
//            if (connect == null) return new ObservableCollection<Recipe>();

//            try
//            {
//                string sqlquery = "SELECT Id, Name, Imagepath FROM Recipe";
//                using (var command = connect.CreateCommand())
//                {
//                    command.CommandText = sqlquery;
//                    using (var reader = await command.ExecuteReaderAsync())
//                    {
//                        var recipe = new ObservableCollection<Recipe>();
//                        while (await reader.ReadAsync())
//                        {
//                            recipe.Add(new Recipe()
//                            {
//                                Id = reader.GetInt32(0),
//                                Name = reader.GetString(1),
//                                ImagePath = reader.GetString(2),
//                                Ingredientlist = await GetIngredientsForRecipe(reader.GetInt32(0), goods)
//                            });

//                        }
//                        return recipe;
//                    }
//                }
//            }
//            catch (SqlException ex)
//            {
//                Debug.WriteLine("SQL Error: " + ex.Message);
//                return new ObservableCollection<Recipe>();
//            }
//            catch (Exception ex)
//            {
//                Debug.WriteLine("Error: " + ex.Message);
//                return new ObservableCollection<Recipe>();
//            }
//            finally
//            {
//                await connect.CloseAsync();
//            }
//        }

//        private static async Task<ObservableCollection<Ingredient>> GetIngredientsForRecipe(int id, ObservableCollection<Goods> goods)
//        {
//            var connect = await ConnectToDatabase(connectionString);
//            if (connect == null) return new ObservableCollection<Ingredient>();
//            try
//            {
//                string sqlquery = "SELECT Goods_Id, Quantity, Unit FROM Ingredient WHERE Recipe_Id = @id";
//                using (var command = connect.CreateCommand())
//                {
//                    command.CommandText = sqlquery;
//                    command.Parameters.AddWithValue("@id", id);
//                    using (var reader = await command.ExecuteReaderAsync())
//                    {
//                        var ingredients = new ObservableCollection<Ingredient>();
//                        while (await reader.ReadAsync())
//                        {
//                            int goodsId = reader.GetInt32(0);
//                            var good = goods.FirstOrDefault(x => x.Id == goodsId);
//                            if (good == null)
//                            {
//                                Debug.WriteLine("Error: Goods with ID " + goodsId + " not found in provided goods collection.");
//                                continue;
//                            }
//                            //Goods good = await GetGoodsById(goodsId);
//                            ingredients.Add(new Ingredient(reader.GetInt32(1), reader.GetString(2), good)
//                            {
//                                //Good = good,
//                                //Quantity = reader.GetInt32(1),
//                                //Unit = reader.GetString(2)
//                            });
//                        }
//                        return ingredients;
//                    }
//                }
//            }
//            catch (SqlException ex)
//            {
//                Debug.WriteLine("SQL Error: " + ex.Message);
//                return new ObservableCollection<Ingredient>();
//            }
//            catch (Exception ex)
//            {
//                Debug.WriteLine("Error: " + ex.Message);
//                return new ObservableCollection<Ingredient>();
//            }
//            finally
//            {
//                await connect.CloseAsync();
//            }
//        }

//        private static async Task<Goods> GetGoodsById(int goodsId)
//        {
//            var connect = await ConnectToDatabase(connectionString);
//            if (connect == null) return null;
//            try
//            {
//                string sqlquery = "SELECT Id, Name, ImagePath, GperDL, GperST FROM Goods WHERE Id = @Id";
//                using (var command = connect.CreateCommand())
//                {
//                    command.CommandText = sqlquery;
//                    command.Parameters.AddWithValue("@Id", goodsId);
//                    using (var reader = await command.ExecuteReaderAsync())
//                    {
//                        if (await reader.ReadAsync())
//                        {
//                            return new Goods
//                            {
//                                Id = reader.GetInt32(0),
//                                Name = reader.GetString(1),
//                                ImagePath = reader.GetString(2),
//                                GramsPerDeciliter = reader.GetInt32(3),
//                                GramsPerStick = reader.GetInt32(4)
//                            };
//                        }
//                        else
//                        {
//                            return null;
//                        }
//                    }
//                }
//            }
//            catch (SqlException ex)
//            {
//                Debug.WriteLine("SQL Error: " + ex.Message);
//                return null;
//            }
//            catch (Exception ex)
//            {
//                Debug.WriteLine("Error: " + ex.Message);
//                return null;
//            }
//            finally
//            {
//                await connect.CloseAsync();
//            }
//        }

//        public static async Task<bool> DeleteGoodsFromDB(Goods selectedGood)
//        {
//            var connect = await ConnectToDatabase(connectionString);
//            if (connect == null) return false;
//            try
//            {
//                string sqlquery = "DELETE FROM Goods WHERE ID = @Id";
//                using (var command = connect.CreateCommand())
//                {
//                    command.CommandText = sqlquery;
//                    command.Parameters.Add("@Id", System.Data.SqlDbType.Int).Value = selectedGood.Id;
//                    int rows = await command.ExecuteNonQueryAsync();
//                    return rows > 0;
//                }
//            }
//            catch (SqlException ex)
//            {
//                Debug.WriteLine($"SQL Error deleting Goods with ID {selectedGood.Id}: {ex.Message}");
//                return false;
//            }
//            catch (Exception ex)
//            {
//                Debug.WriteLine($"Unexpected error deleting Goods with ID {selectedGood.Id}: {ex.Message}");
//                return false;
//            }
//            finally
//            {
//                if (connect != null) await connect.CloseAsync();
//            }
//        }

//        public static async Task<bool> DeleteRecipeFromDB(Recipe selectedRecipe)
//        {
//            var connect = await ConnectToDatabase(connectionString);
//            if (connect == null) return false;
//            try
//            {
//                string sqlquery = "DELETE FROM Recipe WHERE ID = @Id";
//                using (var command = connect.CreateCommand())
//                {
//                    command.CommandText = sqlquery;
//                    command.Parameters.Add("@Id", System.Data.SqlDbType.Int).Value = selectedRecipe.Id;
//                    int rows = await command.ExecuteNonQueryAsync();
//                    return rows > 0;
//                }
//            }
//            catch (SqlException ex)
//            {
//                Debug.WriteLine($"SQL Error deleting Goods with ID {selectedRecipe.Id}: {ex.Message}");
//                return false;
//            }
//            catch (Exception ex)
//            {
//                Debug.WriteLine($"Unexpected error deleting Goods with ID {selectedRecipe.Id}: {ex.Message}");
//                return false;
//            }
//            finally
//            {
//                if (connect != null) await connect.CloseAsync();
//            }
//        }

//        public static async Task<ObservableCollection<MenuItem>> GetAllMenuItemsFromDB()
//        {
//            return new ObservableCollection<MenuItem>();
//        }
//    }
//}

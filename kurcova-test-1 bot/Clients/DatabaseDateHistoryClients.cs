using kurcova_test_1_bot.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Npgsql;
using System.Threading.Tasks;

namespace kurcova_test_1_bot.Clients
{
    public class DatabaseDateHistoryClients
    {
        NpgsqlConnection con = new NpgsqlConnection(Constants.Connect);
        ////додавання
        public async Task InsertDateHistoryAsync(Datehistory datehistory)
        {
            var sql = "insert into public.\"Datehistory\"(\"UserId\", \"Date\", \"Information\")"
                + $"values (@UserId, @Date, @Information)";
            NpgsqlCommand comm = new NpgsqlCommand(sql, con);
            comm.Parameters.AddWithValue("UserId", datehistory.UserId);
            comm.Parameters.AddWithValue("Date", datehistory.Date);
            comm.Parameters.AddWithValue("Information", datehistory.Information);
            await con.OpenAsync();
            await comm.ExecuteNonQueryAsync();
            await con.CloseAsync();
        }


        //виведення всіх подій
        public async Task<List<Datehistory>> SelectDateHistoryAsync(long userId)
        {
            List<Datehistory> dthistory = new List<Datehistory>();
            await con.OpenAsync();
            var sql = "SELECT \"UserId\", \"Date\", \"Information\" FROM public.\"Datehistory\" WHERE \"UserId\" = @UserId";
            NpgsqlCommand comm = new NpgsqlCommand(sql, con);
            comm.Parameters.AddWithValue("@UserId", userId);
            NpgsqlDataReader reader = await comm.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                int userid = reader.GetInt32(0);
                string date = reader.GetString(1);
                string[] information = (string[])reader.GetValue(2);

                dthistory.Add(new Datehistory { UserId = userid, Date = date, Information = information });
            }
            await con.CloseAsync();
            return dthistory;
        }

        //видалення одної події
        public async Task DeleteDateHistoryByDateAndUserIdAsync(string date, long userId)
        {
            await con.OpenAsync();
            var sql = "DELETE FROM public.\"Datehistory\" WHERE \"Date\" = @Date AND \"UserId\" = @UserId";
            NpgsqlCommand comm = new NpgsqlCommand(sql, con);
            comm.Parameters.AddWithValue("Date", date);
            comm.Parameters.AddWithValue("UserId", userId);
            await comm.ExecuteNonQueryAsync();
            await con.CloseAsync();

        }

        //оновлення 
        public async Task UpdateDateHistoryByIdAndDateAsync(string date, long userId, Datehistory datehistory)
        {
            await con.OpenAsync();
            var sql = "UPDATE public.\"Datehistory\" SET \"UserId\" = @UpdatedUserId, \"Date\" = @UpdatedDate, \"Information\" = @UpdatedInformation WHERE \"UserId\" = @UserId AND LOWER(\"Date\") = LOWER(@Date)";
            NpgsqlCommand comm = new NpgsqlCommand(sql, con);
            comm.Parameters.AddWithValue("UpdatedUserId", datehistory.UserId);
            comm.Parameters.AddWithValue("UpdatedDate", datehistory.Date);
            comm.Parameters.AddWithValue("UpdatedInformation", datehistory.Information);
            comm.Parameters.AddWithValue("UserId", userId);
            comm.Parameters.AddWithValue("Date", date);
            await comm.ExecuteNonQueryAsync();
            await con.CloseAsync();
        }

        //виведення однієї події
        public async Task<Datehistory> GetDateHistoryByIdAndDateAsync(long userId, string date)
        {
            await con.OpenAsync();
            var sql = "SELECT \"UserId\", \"Date\", \"Information\" FROM public.\"Datehistory\" WHERE \"UserId\" = @UserId AND LOWER(\"Date\") = LOWER(@Date)";
            NpgsqlCommand comm = new NpgsqlCommand(sql, con);
            comm.Parameters.AddWithValue("@UserId", userId);
            comm.Parameters.AddWithValue("@Date", date);
            NpgsqlDataReader reader = await comm.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                long usersId = reader.GetInt64(0);
                string dates = reader.GetString(1);
                string[] information = (string[])reader.GetValue(2);


                Datehistory dthistory = new Datehistory { UserId = usersId, Date = dates, Information = information };
                await con.CloseAsync();
                return dthistory;
            }

            await con.CloseAsync();
            return null;
        }




    }
}

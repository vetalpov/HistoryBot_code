using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot.Types;
using Telegram.Bot;
using static System.Net.Mime.MediaTypeNames;
using kurcova_test_1_bot.Models;
using Microsoft.Extensions.Logging;
using kurcova_test_1_bot.Models;
using kurcova_test_1_bot.Clients;
using System.Text;
using Microsoft.VisualBasic;

namespace kurcova_test_1_bot
{
    internal class HistoryBot
    {
        public string teams { get; set; }

        TelegramBotClient botClient = new TelegramBotClient("5994324033:AAFmM6r79iT-JE9JFtlF8WRGSNjsZe0VQHI");
        Datehistory datehistory = new Datehistory();
        HistoricalEvents events = new HistoricalEvents();        
        CancellationToken cancellationToken = new CancellationToken();
        ReceiverOptions receiverOptions = new ReceiverOptions { AllowedUpdates = { } };

        public async Task Start()
        {
            botClient.StartReceiving(HandlerUpdateAsync, HandlerError, receiverOptions, cancellationToken);
            var botMe = await botClient.GetMeAsync();
            Console.WriteLine($"{botMe.Username} Bot has just started working");
            Console.ReadKey();
        }

        private Task HandlerError(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            var ErrorMessage = exception switch
            {
                ApiRequestException apiRequestException => $"Помилка в телеграм бот АПІ:\n {apiRequestException.ErrorCode}" +
                $"\n{apiRequestException.Message}",
                _ => exception.ToString()
            };
            Console.WriteLine(ErrorMessage);
            return Task.CompletedTask;
        }

        private async Task HandlerUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            if (update.Type == UpdateType.Message && update?.Message?.Text != null)
            {
                await HandlerMessageAsync(botClient, update.Message);
            }

        }
        
        private async Task HandlerMessageAsync(ITelegramBotClient botClient, Message message)
        {
            if (message.Text == "/start")
            {       
                await botClient.SendTextMessageAsync(message.Chat.Id, "Привіт! Це історичний бот - твій помічник у світі минулого. Він шукає відомі події, має записник для важливих дат, може видаляти, виводити й оновлювати записи. \nДавай разом відкриємо віхи історії!");
                

                ReplyKeyboardMarkup replyKeyboardMarkup = new
                   (
                   new[]
                       {
                            new KeyboardButton[] { "Знайти історичну подію" },                            
                            new KeyboardButton[] { "Записник" }
                       }
                   )
                {
                    ResizeKeyboard = true
                };
                await botClient.SendTextMessageAsync(message.Chat.Id, "Виберіть опцію:", replyMarkup: replyKeyboardMarkup);
                return;
            }
            else if (message.Text == "Знайти історичну подію")
            {
                await botClient.SendTextMessageAsync(message.Chat.Id, "Короткими/ключовими словами опишіть подію:\n(інформацію вказувати англійською) ");
                teams = "EnterStartEvents";

            }
            else if (message.Text == "Записник")
            {
                ReplyKeyboardMarkup replyKeyboardMarkup = new
                   (
                   new[]
                       {
                        new KeyboardButton[] { "Пошук дати", "Переглянути всі дати" },
                        new KeyboardButton[] { "Додати дату", "Видалити дату"},
                        new KeyboardButton[] { "Оновити інформацію"},
                        new KeyboardButton[] { "Назад"},
                       }
                   )
                {
                    ResizeKeyboard = true
                };
                await botClient.SendTextMessageAsync(message.Chat.Id, "Виберіть опцію : ", replyMarkup: replyKeyboardMarkup);
                return;

            }
            else if (message.Text == "Пошук дати")
            {
                await botClient.SendTextMessageAsync(message.Chat.Id, "Введіть дату :");
                teams = "SearchDate";
            }
            else if (message.Text == "Переглянути всі дати")
            {
                datehistory.UserId = message.Chat.Id;               
                DisplayDates(botClient, message, datehistory.UserId);
            }
            else if (message.Text == "Додати дату")
            {
                await botClient.SendTextMessageAsync(message.Chat.Id, "Введіть дату :");
                teams = "AddDate";
                
            }
            else if (message.Text == "Видалити дату")
            {
                await botClient.SendTextMessageAsync(message.Chat.Id, "Введіть дату :");
                teams = "DeleteDate";
            }
            else if (message.Text == "Оновити інформацію")
            {
                await botClient.SendTextMessageAsync(message.Chat.Id, "Введіть дату :");
                teams = "UpdateDate";
            }
            else if (message.Text == "Назад")
            {
                ReplyKeyboardMarkup replyKeyboardMarkup = new
                  (
                  new[]
                      {
                            new KeyboardButton[] { "Знайти історичну подію" },                           
                            new KeyboardButton[] { "Записник" }
                      }
                  )
                {
                    ResizeKeyboard = true
                };
                await botClient.SendTextMessageAsync(message.Chat.Id, "Виберіть опцію:", replyMarkup: replyKeyboardMarkup);
                return;
            }
            else
            {
                if (teams == "EnterStartEvents")
                {
                    events.Event = message.Text;
                    SendEvent(botClient, message, events.Event);
                    //await botClient.SendTextMessageAsync(message.Chat.Id, "Знайдено");

                }
                else if (teams == "AddDate")
                {
                    datehistory.UserId = message.Chat.Id;
                    datehistory.Date = message.Text;
                    await botClient.SendTextMessageAsync(message.Chat.Id, "Введіть інформацію про дату :");
                    teams = "AddDateText";             
                }
                else if (teams == "AddDateText")
                {
                    var text = message.Text;
                    datehistory.Information = text.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
                    AddDate(botClient, message, datehistory.UserId, datehistory.Date, datehistory.Information);
                }
                else if(teams == "SearchDate")
                {
                    datehistory.UserId = message.Chat.Id;
                    datehistory.Date = message.Text;
                    SearchDate(botClient, message, datehistory.UserId, datehistory.Date);
                }
                else if (teams == "DeleteDate")
                {
                    datehistory.UserId = message.Chat.Id;
                    datehistory.Date = message.Text;
                    DeleteDate(botClient, message, datehistory.UserId, datehistory.Date);
                }
                else if (teams == "UpdateDate")
                {
                    datehistory.UserId = message.Chat.Id;
                    datehistory.Date = message.Text;
                    await botClient.SendTextMessageAsync(message.Chat.Id, "Введіть оновлену інформацію про дату :");
                    teams = "UpdateDateText";
                }
                else if (teams == "UpdateDateText")
                {
                    var text = message.Text;
                    datehistory.Information = text.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
                    Updatedate(botClient, message, datehistory.UserId, datehistory.Date, datehistory.Information);
                    teams = null;
                }
                else
                {
                    await botClient.SendTextMessageAsync(message.Chat.Id, $"Error");
                }
            }
        }
        //Events
        private async Task SendEvent(ITelegramBotClient botClient, Message message, string text)
        {
            List<HistoricalEvents> events = await HistoricalEventsClient.GetHistoricalEventsAsync(text);

            if (events.Count > 0)
            {
                string allEvents = "";
                foreach (HistoricalEvents evt in events)
                {
                    allEvents += $"Year: {evt.Year}\n";
                    allEvents += $"Month: {evt.Month}\n";
                    allEvents += $"Day: {evt.Day}\n";
                    allEvents += $"Event: {evt.Event}\n";
                    allEvents += "\n";
                }


                await botClient.SendTextMessageAsync(message.Chat.Id, allEvents);
            }
            else
            {
                await botClient.SendTextMessageAsync(message.Chat.Id, "Не знайдено.");
            }
        }
        private async Task AddDate(ITelegramBotClient botClient, Message message, long userId, string date, string[] information)
        {
            DatabaseDateHistoryClients db = new DatabaseDateHistoryClients();
            Datehistory dateHistory = new Datehistory { UserId = userId, Date = date, Information = information };
            await db.InsertDateHistoryAsync(datehistory);
            await botClient.SendTextMessageAsync(message.Chat.Id, "Ви додали нову дату до записника");
        }



        //Пошук дати
        private async Task SearchDate(ITelegramBotClient botClient, Message message, long userId, string date)
        {
            DatabaseDateHistoryClients db = new DatabaseDateHistoryClients();
            try
            {

                Datehistory datehistory = await db.GetDateHistoryByIdAndDateAsync(userId, date);
                if (datehistory != null)
                {
                    string recipeInfo = $"Дату знайдено! \nДата: {datehistory.Date}\nІнформація: {string.Join(", ", datehistory.Information)}";
                    await botClient.SendTextMessageAsync(message.Chat.Id, recipeInfo);
                }
                else
                {
                    await botClient.SendTextMessageAsync(message.Chat.Id, "Не знайдено.");
                }
            }
            catch (Exception ex)
            {
                await botClient.SendTextMessageAsync(message.Chat.Id, $"Error: {ex.Message}");
            }
        }

        //Виведення дат користувача
        private async Task DisplayDates(ITelegramBotClient botClient, Message message, long userId)
        {
            DatabaseDateHistoryClients db = new DatabaseDateHistoryClients();
            List<Datehistory> datehistory = await db.SelectDateHistoryAsync(userId);

            List<string> dates = new List<string>();

            if (datehistory.Count != 0)
            {
                foreach (Datehistory dt in datehistory)
                {
                    dates.Add($"Дата: {Convert.ToString(dt.Date)}\nІнформація: {string.Join(", ", dt.Information)}\n");
                }
                await botClient.SendTextMessageAsync(message.Chat.Id, $"Ось список ваших дат:\n{string.Join("\n", dates)}");
            }
            else
            {
                await botClient.SendTextMessageAsync(message.Chat.Id, "Не знайдено.");
            }


        }

        //Видалення дат користувача
        private async Task DeleteDate(ITelegramBotClient botClient, Message message, long userId, string date)
        {
            DatabaseDateHistoryClients db = new DatabaseDateHistoryClients();
            try
            {
                await db.DeleteDateHistoryByDateAndUserIdAsync(date, userId);
                await botClient.SendTextMessageAsync(message.Chat.Id, $"Дату видалено.");
            }
            catch (Exception ex)
            {
                await botClient.SendTextMessageAsync(message.Chat.Id, $"Error: {ex.Message}");
            }
        }

        //Оновлення інформації по даті
        private async Task Updatedate(ITelegramBotClient botClient, Message message, long userId, string date, string[] information)
        {
            DatabaseDateHistoryClients db = new DatabaseDateHistoryClients();
            Datehistory updatedDate = new Datehistory{ UserId = userId, Date = date, Information = information };
            try
            {
                await db.UpdateDateHistoryByIdAndDateAsync(date, userId,  updatedDate);
                await botClient.SendTextMessageAsync(message.Chat.Id, "Інформацію оновлено!");
            }
            catch (Exception ex)
            {
                await botClient.SendTextMessageAsync(message.Chat.Id, $"Error: {ex.Message}");
            }
        }




    }
}


using kurcova_test_1_bot;

HistoryBot mybot = new HistoryBot();
try
{
    mybot.Start();
}
catch (Exception ex)
{
    Console.WriteLine(ex.Message);
}

Console.ReadKey();

namespace DurableFunctionsSaga.OrderProcess.Models
{
    public enum OrderStatus
    {
        WaitForPayment,
        PaymentSucessfull,
        PaymentFailed,
        PaymentTimeout,
        Completed,
        Created
    }
}

namespace Automata.Entities
{
    public class Stock : ExchangeTradable
    {
        public override string ToString()
        {
            return Exchange.Code + ":" + Code;
        }
    }
}
namespace OmadaPOS.Libreria.Models;

public class MyCart
{
    public MyCart(double quantity, string name, decimal price, decimal total)
    {
        _quantity = quantity;
        _name = name;
        _price = price;
        _total = total;
    }

    private double _quantity;
    public double Quantity
    {
        get { return _quantity; }
        set { _quantity = value; }
    }
     
    private string _name;
    public string Name
    {
        get { return _name; }
        set
        {
            if (value.Length > 15)
            {
                _name = value.Substring(0, 15);
            }
            else
            {
                _name = value;
            }
        }
    }

    private decimal _price;
    public decimal Price
    {
        get { return _price; }
        set { _price = value; }
    }

    private decimal _total;
    public decimal Total
    {
        get { return _total; }
        set { _total = value; }
    }
}

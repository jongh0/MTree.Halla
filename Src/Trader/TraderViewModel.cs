using GalaSoft.MvvmLight.Command;
using Configuration;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Threading;
using CommonLib.Extension;

namespace Trader
{
    public class TraderViewModel: INotifyPropertyChanged
    {
        private static readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        private ITrader Trader { get; set; }

        private ObservableCollectionEx<string> _accountNumbers;
        public ObservableCollectionEx<string> AccountNumbers
        {
            get { return _accountNumbers; }
            set
            {
                _accountNumbers = value;
                NotifyPropertyChanged(nameof(AccountNumbers));
            }
        }

        private string _selectedAccount;
        public string SelectedAccount
        {
            get { return _selectedAccount; }
            set
            {
                _selectedAccount = value;
                NotifyPropertyChanged(nameof(SelectedAccount));
            }
        }

        private string _originalOrderNumber;
        public string OriginalOrderNumber
        {
            get { return _originalOrderNumber; }
            set
            { 
                _originalOrderNumber = value;
                NotifyPropertyChanged(nameof(OriginalOrderNumber));
                NotifyPropertyChanged(nameof(CanOrder));
            }
        }

        public bool OriginalOrderNumberEnabled
        {
            get
            {
                switch (OrderType)
                {
                    case OrderTypes.BuyNew:
                    case OrderTypes.SellNew:
                        return false;
                    default:
                        return true;
                }
            }
        }

        private string _code = "005930";
        public string Code
        {
            get { return _code; }
            set
            {
                _code = value;

                NotifyPropertyChanged(nameof(Code));
                NotifyPropertyChanged(nameof(CanOrder));
            }
        }

        private long _price = 2410000;
        public long Price
        {
            get { return _price; }
            set
            {
                _price = value;
                NotifyPropertyChanged(nameof(Price));
                NotifyPropertyChanged(nameof(CanOrder));
            }
        }

        private long _quantity = 1;
        public long Quantity
        {
            get { return _quantity; }
            set
            {
                _quantity = value;
                NotifyPropertyChanged(nameof(Quantity));
                NotifyPropertyChanged(nameof(CanOrder));
            }
        }

        private OrderTypes _orderType = OrderTypes.BuyNew;
        public OrderTypes OrderType
        {
            get { return _orderType; }
            set
            {
                _orderType = value;
                NotifyPropertyChanged(nameof(OrderType));
                NotifyPropertyChanged(nameof(CanOrder));
                NotifyPropertyChanged(nameof(OriginalOrderNumberEnabled));
            }
        }

        private string _traderStatus;
        public string TraderState
        {
            get { return _traderStatus; }
            set
            {
                _traderStatus = value;
                NotifyPropertyChanged(nameof(TraderState));
            }
        }

        private RelayCommand _orderCommand;
        public ICommand OrderCommand => _orderCommand ?? (_orderCommand = new RelayCommand(() => ExecuteOrder()));

        public bool CanOrder
        {
            get
            {
                if (string.IsNullOrEmpty(Code) == true ||
                    Quantity <= 0 ||
                    Price <= 0)
                    return false;

                switch (OrderType)
                {
                    case OrderTypes.BuyModify:
                    case OrderTypes.BuyCancel:
                    case OrderTypes.SellModify:
                    case OrderTypes.SellCancel:
                        if (string.IsNullOrEmpty(OriginalOrderNumber) == true)
                            return false;
                        break;
                }

                return true;
            }
        }

        public void ExecuteOrder()
        {

            var order = new Order();
            order.AccountNumber = SelectedAccount;
            order.AccountPassword = Config.General.AccountPw;
            order.Code = Code;
            order.OrderType = OrderType;
            order.Quantity = Quantity;
            order.Price = Price;

            Trader.MakeOrder(order);
        }

        public TraderViewModel(ITrader trader)
        {
            try
            {
                Trader = trader ?? throw new ArgumentNullException(nameof(trader));
                Trader.StateNotified += Trader_StateNotified;

                AccountNumbers = new ObservableCollectionEx<string>();

                Task.Run(() =>
                {
                    var accounts = trader.GetAccountList();
                    if (accounts == null) return;

                    foreach (string account in accounts)
                    {
                        _logger.Info($"Accout Number: {account}");
                        AccountNumbers.Add(account);
                    }

                    SelectedAccount = AccountNumbers[0];
                });
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
        }

        private void Trader_StateNotified(string state)
        {
            TraderState = state;
        }

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        #endregion
    }
}

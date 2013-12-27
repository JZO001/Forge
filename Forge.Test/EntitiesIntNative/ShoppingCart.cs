using System;
using System.Collections.Generic;
using System.Diagnostics;
using Forge.ORM.NHibernateExtension.Model;
using NHibernate.Mapping.Attributes;

namespace Forge.Test.EntitiesIntNative
{

    [Serializable]
    [Class]
    public class ShoppingCart : EntityBaseInt32NativeId
    {

        /// <summary>
        /// The customer
        /// </summary>
        [ManyToOne(0, Name = "customer", ClassType = typeof(Customer), Cascade = "none", Lazy = Laziness.Proxy)]
        [Column(1, Name = "customerId")]
        private Customer customer = null;

        /// <summary>
        /// The selected products
        /// </summary>
        [Set(0, Name = "products", Cascade = "none", Generic = true, Lazy = CollectionLazy.True, Table = "ShoppingCart_Product_Switch")]
        [Key(1)]
        [Column(2, Name = "shoppingCartId")]
        [ManyToMany(3, NotFound = NotFoundMode.Exception, ClassType = typeof(Product))]
        [Column(4, Name = "productId")]
        private ISet<Product> products = new HashSet<Product>();

        /// <summary>
        /// Initializes a new instance of the <see cref="ShoppingCart"/> class.
        /// </summary>
        public ShoppingCart()
        {
        }

        /// <summary>
        /// Gets or sets the customer.
        /// </summary>
        /// <value>
        /// The customer.
        /// </value>
        [DebuggerHidden]
        public virtual Customer Customer
        {
            get { return customer; }
            set
            {
                OnPropertyChanging("Customer");
                customer = value;
                OnPropertyChanged("Customer");
            }
        }

        /// <summary>
        /// Gets or sets the products.
        /// </summary>
        /// <value>
        /// The products.
        /// </value>
        [DebuggerHidden]
        public virtual ISet<Product> Products
        {
            get { return products; }
            set
            {
                OnPropertyChanging("Products");
                products = value;
                OnPropertyChanged("Products");
            }
        }

    }

}

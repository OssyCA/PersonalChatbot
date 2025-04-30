import { useState, useEffect } from "react";

// Separate components for better structure
const ProductRow = ({ product, onBuyClick }) => (
  <div className="product-row">
    <div className="product-cell product-name">
      <div className="product-icon">
        {product.name.includes("Message") ? (
          <svg
            viewBox="0 0 24 24"
            width="24"
            height="24"
            stroke="currentColor"
            strokeWidth="2"
            fill="none"
            strokeLinecap="round"
            strokeLinejoin="round"
          >
            <path d="M21 11.5a8.38 8.38 0 0 1-.9 3.8 8.5 8.5 0 0 1-7.6 4.7 8.38 8.38 0 0 1-3.8-.9L3 21l1.9-5.7a8.38 8.38 0 0 1-.9-3.8 8.5 8.5 0 0 1 4.7-7.6 8.38 8.38 0 0 1 3.8-.9h.5a8.48 8.48 0 0 1 8 8v.5z"></path>
          </svg>
        ) : (
          <svg
            viewBox="0 0 24 24"
            width="24"
            height="24"
            stroke="currentColor"
            strokeWidth="2"
            fill="none"
            strokeLinecap="round"
            strokeLinejoin="round"
          >
            <rect x="2" y="3" width="20" height="14" rx="2" ry="2"></rect>
            <line x1="8" y1="21" x2="16" y2="21"></line>
            <line x1="12" y1="17" x2="12" y2="21"></line>
          </svg>
        )}
      </div>
      {product.name}
    </div>
    <div className="product-cell product-price">
      <div>{product.price}</div>
      {product.period && <div className="price-period">{product.period}</div>}
    </div>
    <div className="product-cell product-created">{product.created}</div>
    <div className="product-cell product-updated">{product.updated}</div>
    <div className="product-cell product-action">
      <button
        className="btn btn-primary btn-sm"
        onClick={() => onBuyClick(product)}
      >
        Köp
      </button>
    </div>
  </div>
);

const LoadingSpinner = ({ message }) => (
  <div className="loading-container">
    <div className="spinner large-spinner"></div>
    <p>{message || "Laddar..."}</p>
  </div>
);

const CustomerForm = ({ customerInfo, onInputChange, product }) => (
  <div className="customer-form">
    <p>Fyll i dina uppgifter för att fortsätta med köpet:</p>

    <div className="form-group">
      <label htmlFor="name">Namn</label>
      <input
        type="text"
        id="name"
        name="name"
        className="form-input"
        value={customerInfo.name}
        onChange={onInputChange}
        required
      />
    </div>

    <div className="form-group">
      <label htmlFor="email">E-post</label>
      <input
        type="email"
        id="email"
        name="email"
        className="form-input"
        value={customerInfo.email}
        onChange={onInputChange}
        required
      />
    </div>

    <div className="selected-product-summary">
      <h4>Produkt:</h4>
      <div className="product-info">
        <div className="product-name">{product.name}</div>
        <div className="product-price">
          {product.price}{" "}
          {product.period && `(${product.period.toLowerCase()})`}
        </div>
      </div>
    </div>
  </div>
);

const PurchaseModal = ({
  product,
  showCustomerForm,
  customerInfo,
  onInputChange,
  onCancel,
  onConfirm,
  onContinue,
  loading,
}) => (
  <div className="modal-overlay">
    <div className="purchase-modal">
      <div className="modal-header">
        <h2>{showCustomerForm ? "Kundinformation" : "Bekräfta köp"}</h2>
        <button className="close-button" onClick={onCancel}>
          ×
        </button>
      </div>

      <div className="modal-body">
        {showCustomerForm ? (
          <CustomerForm
            customerInfo={customerInfo}
            onInputChange={onInputChange}
            product={product}
          />
        ) : (
          <>
            <p>Du är på väg att köpa:</p>
            <div className="selected-product">
              <h3>{product.name}</h3>
              <p className="product-price">{product.price}</p>
              {product.period && (
                <p className="price-period">{product.period}</p>
              )}
              <p className="product-description">{product.description}</p>
            </div>
          </>
        )}
      </div>

      <div className="modal-footer">
        <button className="btn btn-outline" onClick={onCancel}>
          Avbryt
        </button>

        {showCustomerForm ? (
          <button
            className="btn btn-primary"
            onClick={onConfirm}
            disabled={loading || !customerInfo.name || !customerInfo.email}
          >
            {loading ? (
              <>
                <span className="spinner"></span>
                Bearbetar...
              </>
            ) : (
              "Gå till betalning"
            )}
          </button>
        ) : (
          <button className="btn btn-primary" onClick={onContinue}>
            Fortsätt
          </button>
        )}
      </div>
    </div>
  </div>
);

// Stripe API Service - Separates API calls from component logic
const StripeService = {
  async getProducts() {
    try {
      console.log("Hämtar produkter från Stripe API");
      const response = await fetch(
        "https://localhost:7289/api/stripe/GetProducts",
        {
          method: "GET",
          credentials: "include",
          headers: {
            Accept: "application/json",
          },
        }
      );

      if (!response.ok) {
        throw new Error(
          `Kunde inte hämta produkter: ${response.status} ${response.statusText}`
        );
      }

      const responseText = await response.text();
      const products = JSON.parse(responseText);
      return { success: true, products };
    } catch (error) {
      console.error("Fel vid hämtning av produkter:", error);
      return {
        success: false,
        error: error.message || "Ett fel uppstod vid hämtning av produkter",
      };
    }
  },

  async createCustomer(customerInfo) {
    try {
      const response = await fetch(
        "https://localhost:7289/api/stripe/CreateCustomer",
        {
          method: "POST",
          headers: {
            "Content-Type": "application/json",
          },
          body: JSON.stringify(customerInfo),
          credentials: "include",
        }
      );

      if (!response.ok) {
        throw new Error("Kunde inte skapa kund");
      }

      const customer = await response.json();
      return { success: true, customer };
    } catch (error) {
      console.error("Fel vid skapande av kund:", error);
      return {
        success: false,
        error: error.message || "Ett fel uppstod vid skapande av kund",
      };
    }
  },

  async processPayment(priceId) {
    try {
      console.log("Anropar Stripe Pay API med priceId:", priceId);
      const response = await fetch(
        `https://localhost:7289/api/stripe/Pay?priceId=${priceId}`,
        {
          method: "POST",
          credentials: "include",
        }
      );

      if (!response.ok) {
        throw new Error("Kunde inte behandla betalningen");
      }

      const stripeUrl = await response.text();
      const cleanUrl = stripeUrl.replace(/^"|"$/g, "").trim();

      return { success: true, redirectUrl: cleanUrl };
    } catch (error) {
      console.error("Betalningsfel:", error);
      return {
        success: false,
        error: error.message || "Ett fel uppstod vid betalningen",
      };
    }
  },
};

// Helper functions for formatting products
const formatStripeProducts = (products) => {
  return products.data.map((product) => {
    const price = product.default_price;
    return {
      id: product.id,
      priceId: price.id,
      name: product.name,
      price: `${(price.unit_amount / 100).toLocaleString("sv-SE")} kr`,
      period: price.recurring ? `Per ${price.recurring.interval}` : null,
      created: new Date(product.created * 1000).toLocaleDateString("sv-SE"),
      updated: new Date(product.updated * 1000).toLocaleDateString("sv-SE"),
      description: product.description || "Ingen beskrivning tillgänglig",
    };
  });
};

// Main component
const ProductPage = () => {
  const [products, setProducts] = useState([]);
  const [selectedProduct, setSelectedProduct] = useState(null);
  const [loading, setLoading] = useState(false);
  const [productLoading, setProductLoading] = useState(true);
  const [message, setMessage] = useState({ text: "", type: "" });
  const [customerInfo, setCustomerInfo] = useState({ name: "", email: "" });
  const [showCustomerForm, setShowCustomerForm] = useState(false);

  // Fetch products when the component loads
  useEffect(() => {
    const loadProducts = async () => {
      setProductLoading(true);
      try {
        const result = await StripeService.getProducts();

        if (result.success && result.products && result.products.data) {
          const formattedProducts = formatStripeProducts(result.products);
          setProducts(formattedProducts);
        } else {
          // No products found
          setProducts([]);

          if (!result.success) {
            console.warn(
              "Kunde inte hämta produkter från Stripe API",
              result.error
            );
            setMessage({
              text: "Kunde inte ladda produkter från servern. Kontakta support.",
              type: "error",
            });
          }
        }
      } catch (error) {
        console.error("Fel vid inladdning av produkter:", error);
        setMessage({
          text: "Kunde inte ladda produkter. Försök igen senare eller kontakta support.",
          type: "error",
        });
        setProducts([]);
      } finally {
        setProductLoading(false);
      }
    };

    loadProducts();
  }, []);

  const handleCustomerInfoChange = (e) => {
    setCustomerInfo({
      ...customerInfo,
      [e.target.name]: e.target.value,
    });
  };

  const handleBuyClick = (product) => {
    setSelectedProduct(product);
    setShowCustomerForm(false); // First show confirmation view
  };

  const handleContinueToCustomerForm = () => {
    setShowCustomerForm(true);
  };

  const handleCancelPurchase = () => {
    setSelectedProduct(null);
    setShowCustomerForm(false);
    setCustomerInfo({ name: "", email: "" });
  };

  const handleConfirmPurchase = async () => {
    if (!selectedProduct) return;

    setLoading(true);
    setMessage({ text: "Förbereder betalning...", type: "info" });

    try {
      // Create customer if we have customer information
      if (customerInfo.name && customerInfo.email) {
        const customerResult = await StripeService.createCustomer(customerInfo);
        if (!customerResult.success) {
          throw new Error(`Kunde inte skapa kund: ${customerResult.error}`);
        }
      }

      // Show message about redirecting to Stripe
      setMessage({
        text: "Omdirigerar till betalningssidan...",
        type: "success",
      });

      // Process payment
      setTimeout(async () => {
        try {
          const paymentResult = await StripeService.processPayment(
            selectedProduct.priceId
          );

          if (paymentResult.success) {
            window.location.href = paymentResult.redirectUrl;
          } else {
            throw new Error(paymentResult.error);
          }

          // Code below only runs if the redirect fails
          setLoading(false);
        } catch (error) {
          setMessage({
            text: `Kunde inte dirigera till betalningssidan: ${error.message}`,
            type: "error",
          });
          setLoading(false);
        }
      }, 1000);
    } catch (error) {
      console.error("Betalningsfel:", error);
      setMessage({
        text: `Betalningsfel: ${error.message}`,
        type: "error",
      });
      setLoading(false);
    }
  };

  return (
    <div className="product-page">
      <div className="product-header">
        <h1>Produkter</h1>
        <p>Välj en produkt för att köpa</p>
      </div>

      {message.text && (
        <div className={`message ${message.type}-message`}>{message.text}</div>
      )}

      <div className="product-table">
        {productLoading ? (
          <LoadingSpinner message="Laddar produkter..." />
        ) : products.length === 0 ? (
          <div className="no-products-message">
            Inga produkter tillgängliga just nu.
          </div>
        ) : (
          <>
            <div className="product-table-header">
              <div className="product-header-item product-name">Namn</div>
              <div className="product-header-item product-price">Pris</div>
              <div className="product-header-item product-created">Skapad</div>
              <div className="product-header-item product-updated">
                Uppdaterad
              </div>
              <div className="product-header-item product-action">Köp</div>
            </div>
            <div className="product-table-body">
              {products.map((product) => (
                <ProductRow
                  key={product.id}
                  product={product}
                  onBuyClick={handleBuyClick}
                />
              ))}
            </div>
          </>
        )}
      </div>

      {selectedProduct && (
        <PurchaseModal
          product={selectedProduct}
          showCustomerForm={showCustomerForm}
          customerInfo={customerInfo}
          onInputChange={handleCustomerInfoChange}
          onCancel={handleCancelPurchase}
          onConfirm={handleConfirmPurchase}
          onContinue={handleContinueToCustomerForm}
          loading={loading}
        />
      )}
    </div>
  );
};

export default ProductPage;

import { useState, useEffect } from "react";

const EnhancedProductPage = () => {
  const [products, setProducts] = useState([]);
  const [selectedProduct, setSelectedProduct] = useState(null);
  const [loading, setLoading] = useState(false);
  const [productLoading, setProductLoading] = useState(true);
  const [message, setMessage] = useState({ text: "", type: "" });
  const [customerInfo, setCustomerInfo] = useState({ name: "", email: "" });
  const [showCustomerForm, setShowCustomerForm] = useState(false);

  // Stripe integration functions
  const handlePurchaseWithStripe = async (priceId) => {
    try {
      console.log("Anropar Stripe Pay API med priceId:", priceId);
      const response = await fetch(
        `https://localhost:7289/api/stripe/Pay?priceId=${priceId}`,
        {
          method: "POST",
          credentials: "include", // För att inkludera cookies
        }
      );

      if (!response.ok) {
        console.error("API-fel:", response.status, response.statusText);
        throw new Error("Kunde inte behandla betalningen");
      }

      // Stripe returnerar en checkout URL som vi redirectar till
      const stripeUrl = await response.text();
      console.log("Mottagen Stripe URL:", stripeUrl);

      // Säkerställ att URL:en är korrekt formatterad (ta bort citattecken om sådana finns)
      const cleanUrl = stripeUrl.replace(/^"|"$/g, "").trim();
      console.log("Rengjord URL för redirect:", cleanUrl);

      // Forcera en redirect till Stripe checkout
      window.location.href = cleanUrl;

      return { success: true };
    } catch (error) {
      console.error("Betalningsfel:", error);
      return {
        success: false,
        error: error.message || "Ett fel uppstod vid betalningen",
      };
    }
  };

  const fetchStripeProducts = async () => {
    try {
      console.log("Försöker hämta produkter från Stripe API");
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

      console.log("Produktsvar mottaget, status:", response.status);

      if (!response.ok) {
        throw new Error(
          `Kunde inte hämta produkter: ${response.status} ${response.statusText}`
        );
      }

      // Försök hämta som JSON
      const responseText = await response.text();
      console.log("Svarstext:", responseText.substring(0, 100) + "...");

      let products;
      try {
        products = JSON.parse(responseText);
      } catch (jsonError) {
        console.error("Kunde inte tolka JSON:", jsonError);
        throw new Error("Ogiltigt svarsformat från API");
      }

      console.log(
        "Produkter framgångsrikt hämtade:",
        products.data ? products.data.length : "ingen data"
      );
      return { success: true, products };
    } catch (error) {
      console.error("Fel vid hämtning av produkter:", error);
      return {
        success: false,
        error: error.message || "Ett fel uppstod vid hämtning av produkter",
      };
    }
  };

  const createStripeCustomer = async (customerInfo) => {
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
  };

  // Hämta produkter när komponenten laddas
  useEffect(() => {
    const loadProducts = async () => {
      setProductLoading(true);
      try {
        // Försök först att hämta produkter från Stripe
        const result = await fetchStripeProducts();

        if (result.success && result.products && result.products.data) {
          // Formatera Stripe-produkter för vår UI
          const formattedProducts = result.products.data.map((product) => {
            const price = product.default_price;
            return {
              id: product.id,
              priceId: price.id,
              name: product.name,
              price: `${(price.unit_amount / 100).toLocaleString("sv-SE")} kr`,
              period: price.recurring
                ? `Per ${price.recurring.interval}`
                : null,
              created: new Date(product.created * 1000).toLocaleDateString(
                "sv-SE"
              ),
              updated: new Date(product.updated * 1000).toLocaleDateString(
                "sv-SE"
              ),
              description:
                product.description || "Ingen beskrivning tillgänglig",
            };
          });
          setProducts(formattedProducts);
        } else {
          // Fallback till exempeldata
          setProducts([
            {
              id: "prod_test1",
              priceId: "price_test1",
              name: "Send Messages",
              price: "15,00 kr",
              created: "29 apr.",
              updated: "29 apr.",
              description: "Köp meddelanden att använda i tjänsten",
            },
            {
              id: "prod_test2",
              priceId: "price_test2",
              name: "Basic plan",
              price: "100,00 kr",
              period: "Per månad",
              created: "29 apr.",
              updated: "29 apr.",
              description:
                "Prenumerera på vår grundläggande tjänst med alla standardfunktioner",
            },
          ]);

          if (!result.success) {
            console.warn(
              "Kunde inte hämta produkter från Stripe, använder exempeldata",
              result.error
            );
          }
        }
      } catch (error) {
        console.error("Fel vid inladdning av produkter:", error);
        setMessage({
          text: "Kunde inte ladda produkter. Använder exempeldata.",
          type: "warning",
        });

        // Fallback till exempeldata
        setProducts([
          {
            id: "prod_test1",
            priceId: "price_test1",
            name: "Send Messages",
            price: "15,00 kr",
            created: "29 apr.",
            updated: "29 apr.",
            description: "Köp meddelanden att använda i tjänsten",
          },
          {
            id: "prod_test2",
            priceId: "price_test2",
            name: "Basic plan",
            price: "100,00 kr",
            period: "Per månad",
            created: "29 apr.",
            updated: "29 apr.",
            description:
              "Prenumerera på vår grundläggande tjänst med alla standardfunktioner",
          },
        ]);
      } finally {
        setProductLoading(false);
      }
    };

    loadProducts();
  }, []);

  const handleBuyClick = (product) => {
    setSelectedProduct(product);
    // Visa kundformulär för betalningsprocessen
    setShowCustomerForm(true);
  };

  const handleCustomerInfoChange = (e) => {
    setCustomerInfo({
      ...customerInfo,
      [e.target.name]: e.target.value,
    });
  };

  const handleConfirmPurchase = async () => {
    if (!selectedProduct) return;

    setLoading(true);
    setMessage({ text: "", type: "" });

    try {
      // Visa meddelande om att vi förbereder betalningen
      setMessage({
        text: "Förbereder betalning...",
        type: "info",
      });

      // 1. Skapa kund om vi har kundinformation
      if (customerInfo.name && customerInfo.email) {
        console.log("Skapar kund med information:", customerInfo);
        const customerResult = await createStripeCustomer(customerInfo);
        if (!customerResult.success) {
          throw new Error(`Kunde inte skapa kund: ${customerResult.error}`);
        }
        console.log("Kund skapad framgångsrikt");
      }

      // 2. Hantera betalning med Stripe - här skickar vi priceId direkt
      console.log("Påbörjar betalningsprocess för produkt:", selectedProduct);

      // Visa meddelande om att vi omdirigerar till Stripe
      setMessage({
        text: "Omdirigerar till betalningssidan...",
        type: "success",
      });

      // Använd setTimeout för att säkerställa att redirecten sker efter att UI har uppdaterats
      setTimeout(async () => {
        try {
          // Om vi använder demodata, visa ett demo-meddelande istället för att försöka betala
          if (
            selectedProduct.priceId === "price_test1" ||
            selectedProduct.priceId === "price_test2"
          ) {
            alert(
              "Detta är en demoversion. I en riktig implementation skulle du nu dirigeras till Stripe Checkout."
            );
            setLoading(false);
            setSelectedProduct(null);
            setShowCustomerForm(false);
            return;
          }

          // Anropa Stripe för faktisk betalning
          await handlePurchaseWithStripe(selectedProduct.priceId);
          // Obs! Koden nedan körs endast om redirecten till Stripe misslyckas
          setLoading(false);
        } catch (error) {
          console.error("Fel vid redirect till Stripe:", error);
          setLoading(false);
          setMessage({
            text: `Kunde inte dirigera till betalningssidan: ${error.message}`,
            type: "error",
          });
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

  const handleCancelPurchase = () => {
    setSelectedProduct(null);
    setShowCustomerForm(false);
    setCustomerInfo({ name: "", email: "" });
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

      <div className="product-filter-bar">
        <div className="filter-options">
          <span className="filter-label">Alla</span>
          <span className="filter-count">{products.length}</span>
        </div>
        <div className="filter-options">
          <span className="filter-label">Aktiv</span>
          <span className="filter-count">{products.length}</span>
        </div>
        <button className="btn btn-outline btn-sm filter-remove-button">
          Ta bort filter
        </button>
      </div>

      <div className="product-table">
        <div className="product-table-header">
          <div className="product-header-item product-name">Namn</div>
          <div className="product-header-item product-price">Priser</div>
          <div className="product-header-item product-created">Skapad</div>
          <div className="product-header-item product-updated">Uppdaterad</div>
          <div className="product-header-item product-action"></div>
        </div>

        <div className="product-table-body">
          {productLoading ? (
            <div className="loading-container">
              <div className="spinner large-spinner"></div>
              <p>Laddar produkter...</p>
            </div>
          ) : products.length === 0 ? (
            <div className="no-products">Inga produkter tillgängliga</div>
          ) : (
            products.map((product) => (
              <div className="product-row" key={product.id}>
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
                        <rect
                          x="2"
                          y="3"
                          width="20"
                          height="14"
                          rx="2"
                          ry="2"
                        ></rect>
                        <line x1="8" y1="21" x2="16" y2="21"></line>
                        <line x1="12" y1="17" x2="12" y2="21"></line>
                      </svg>
                    )}
                  </div>
                  {product.name}
                </div>
                <div className="product-cell product-price">
                  <div>{product.price}</div>
                  {product.period && (
                    <div className="price-period">{product.period}</div>
                  )}
                </div>
                <div className="product-cell product-created">
                  {product.created}
                </div>
                <div className="product-cell product-updated">
                  {product.updated}
                </div>
                <div className="product-cell product-action">
                  <button
                    className="btn btn-primary btn-sm"
                    onClick={() => handleBuyClick(product)}
                  >
                    Köp
                  </button>
                </div>
              </div>
            ))
          )}
        </div>
      </div>

      {selectedProduct && (
        <div className="modal-overlay">
          <div className="purchase-modal">
            <div className="modal-header">
              <h2>{showCustomerForm ? "Kundinformation" : "Bekräfta köp"}</h2>
              <button className="close-button" onClick={handleCancelPurchase}>
                ×
              </button>
            </div>

            <div className="modal-body">
              {showCustomerForm ? (
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
                      onChange={handleCustomerInfoChange}
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
                      onChange={handleCustomerInfoChange}
                      required
                    />
                  </div>

                  <div className="selected-product-summary">
                    <h4>Produkt:</h4>
                    <div className="product-info">
                      <div className="product-name">{selectedProduct.name}</div>
                      <div className="product-price">
                        {selectedProduct.price}{" "}
                        {selectedProduct.period &&
                          `(${selectedProduct.period.toLowerCase()})`}
                      </div>
                    </div>
                  </div>
                </div>
              ) : (
                <>
                  <p>Du är på väg att köpa:</p>
                  <div className="selected-product">
                    <h3>{selectedProduct.name}</h3>
                    <p className="product-price">{selectedProduct.price}</p>
                    {selectedProduct.period && (
                      <p className="price-period">{selectedProduct.period}</p>
                    )}
                    <p className="product-description">
                      {selectedProduct.description}
                    </p>
                  </div>
                </>
              )}
            </div>

            <div className="modal-footer">
              <button
                className="btn btn-outline"
                onClick={handleCancelPurchase}
              >
                Avbryt
              </button>

              {showCustomerForm ? (
                <button
                  className="btn btn-primary"
                  onClick={handleConfirmPurchase}
                  disabled={
                    loading || !customerInfo.name || !customerInfo.email
                  }
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
                <button
                  className="btn btn-primary"
                  onClick={() => setShowCustomerForm(true)}
                >
                  Fortsätt
                </button>
              )}
            </div>
          </div>
        </div>
      )}

      <style jsx>{`
        .product-page {
          width: 100%;
          max-width: 1000px;
          margin: 0 auto;
          padding: 20px;
          background-color: var(--dark-panel-bg);
          border-radius: var(--border-radius-lg);
          box-shadow: var(--shadow-lg);
        }

        .product-header {
          margin-bottom: 24px;
        }

        .product-header h1 {
          margin-bottom: 8px;
        }

        .product-header p {
          color: var(--text-muted);
        }

        .product-filter-bar {
          display: flex;
          margin-bottom: 20px;
          padding: 12px 16px;
          background-color: var(--darker-panel-bg);
          border-radius: var(--border-radius-md);
        }

        .filter-options {
          display: flex;
          align-items: center;
          margin-right: 24px;
        }

        .filter-label {
          margin-right: 8px;
        }

        .filter-count {
          display: inline-flex;
          align-items: center;
          justify-content: center;
          width: 24px;
          height: 24px;
          background-color: var(--primary-color);
          color: white;
          border-radius: 50%;
          font-size: 0.8rem;
        }

        .filter-remove-button {
          margin-left: auto;
        }

        .product-table {
          width: 100%;
          background-color: var(--darker-panel-bg);
          border-radius: var(--border-radius-md);
          overflow: hidden;
          position: relative;
          min-height: 200px;
        }

        .product-table-header {
          display: grid;
          grid-template-columns: 2fr 1fr 1fr 1fr 1fr;
          padding: 16px;
          background-color: rgba(0, 0, 0, 0.2);
          font-weight: 600;
          border-bottom: 1px solid var(--divider-color);
        }

        .product-row {
          display: grid;
          grid-template-columns: 2fr 1fr 1fr 1fr 1fr;
          padding: 16px;
          border-bottom: 1px solid var(--divider-color);
          align-items: center;
        }

        .product-row:last-child {
          border-bottom: none;
        }

        .product-name {
          display: flex;
          align-items: center;
        }

        .product-icon {
          width: 40px;
          height: 40px;
          background-color: var(--primary-color);
          border-radius: var(--border-radius-md);
          display: flex;
          align-items: center;
          justify-content: center;
          margin-right: 12px;
          color: white;
        }

        .price-period {
          font-size: 0.8rem;
          color: var(--text-muted);
        }

        .loading-container {
          display: flex;
          flex-direction: column;
          align-items: center;
          justify-content: center;
          padding: 40px;
          text-align: center;
          color: var(--text-muted);
        }

        .large-spinner {
          width: 48px;
          height: 48px;
          margin-bottom: 16px;
        }

        .no-products {
          padding: 30px;
          text-align: center;
          color: var(--text-muted);
        }

        .modal-overlay {
          position: fixed;
          top: 0;
          left: 0;
          right: 0;
          bottom: 0;
          background-color: rgba(0, 0, 0, 0.7);
          display: flex;
          align-items: center;
          justify-content: center;
          z-index: 1000;
        }

        .purchase-modal {
          width: 90%;
          max-width: 500px;
          background-color: var(--dark-panel-bg);
          border-radius: var(--border-radius-lg);
          box-shadow: var(--shadow-lg);
          overflow: hidden;
        }

        .modal-header {
          padding: 16px 20px;
          border-bottom: 1px solid var(--divider-color);
          display: flex;
          justify-content: space-between;
          align-items: center;
        }

        .modal-header h2 {
          margin-bottom: 0;
        }

        .close-button {
          background: none;
          border: none;
          font-size: 24px;
          color: var(--text-muted);
          cursor: pointer;
        }

        .close-button:hover {
          color: var(--text-light);
        }

        .modal-body {
          padding: 20px;
        }

        .selected-product {
          background-color: var(--darker-panel-bg);
          border-radius: var(--border-radius-md);
          padding: 16px;
          margin-top: 12px;
        }

        .selected-product h3 {
          margin-bottom: 8px;
          color: var(--text-light);
        }

        .product-description {
          color: var(--text-muted);
          margin-top: 8px;
        }

        .modal-footer {
          padding: 16px 20px;
          border-top: 1px solid var(--divider-color);
          display: flex;
          justify-content: flex-end;
          gap: 12px;
        }

        /* Customer form styles */
        .customer-form {
          margin-bottom: 16px;
        }

        .form-group {
          margin-bottom: 16px;
        }

        .form-group label {
          display: block;
          margin-bottom: 8px;
          font-weight: 500;
        }

        .selected-product-summary {
          margin-top: 24px;
          padding-top: 16px;
          border-top: 1px solid var(--divider-color);
        }

        .selected-product-summary h4 {
          margin-bottom: 12px;
          font-size: 1rem;
        }

        .product-info {
          background-color: var(--darker-panel-bg);
          border-radius: var(--border-radius-md);
          padding: 12px;
        }

        .product-info .product-name {
          font-weight: 600;
          margin-bottom: 4px;
        }

        .product-info .product-price {
          color: var(--text-muted);
          font-size: 0.9rem;
        }

        /* Responsive styles */
        @media (max-width: 768px) {
          .product-table-header,
          .product-row {
            grid-template-columns: 2fr 1fr 1fr;
          }

          .product-created,
          .product-updated {
            display: none;
          }
        }

        @media (max-width: 480px) {
          .product-table-header,
          .product-row {
            grid-template-columns: 1fr auto;
          }

          .product-price {
            display: none;
          }
        }
      `}</style>
    </div>
  );
};

export default EnhancedProductPage;

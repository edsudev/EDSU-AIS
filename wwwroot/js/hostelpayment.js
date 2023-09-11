
function makePayment() {
    const publicKey = document.querySelector('input[name="public_key"]').value;
    const email = document.getElementById('email').value;
    const amount = parseFloat(document.getElementById('amount').value) / 100;
    const ref = document.getElementById('ref').value;

    const paymentData = {
        public_key: publicKey,
        tx_ref: ref,
        amount: amount * 100, // Convert amount to kobo (NGN currency)
        currency: "NGN",
        payment_options: "card, banktransfer, ussd",
        redirect_url: "https://localhost:2222/hostels/RaveRedirect",
        customer: {
            email: email,
        },
        customizations: {
            title: "Edo State University, Uzairue",
            description: "Rave Checkout",
            logo: "https://old.edouniversity.edu.ng/uploads/settings/logo.png",
        },
    };

    // Call the FlutterwaveCheckout function
    FlutterwaveCheckout(paymentData);
  }
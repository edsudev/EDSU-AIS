//// Add an event listener to your payment form
//const paymentForm = document.getElementById('paymentForm');
//paymentForm.addEventListener("submit", function (e) {
//    e.preventDefault();
//    getFlutterwaveApiKeyAndMakePayment();
//});

//function getFlutterwaveApiKeyAndMakePayment() {
//    // Make an AJAX request to retrieve the Flutterwave API key
//    fetch('/wallets/GetRavePaymentKey')
//        .then(response => response.json())
//        .then(data => {
//            const publicKey = data; // Assuming the API key is returned directly

//            const email = document.getElementById('email').value;
//            const amount = parseFloat(document.getElementById('amount').value);
//            const ref = document.getElementById('ref').value;

//            // Define the payment data for Flutterwave
//            const paymentData = {
//                public_key: publicKey,
//                tx_ref: ref,
//                amount: amount,
//                currency: "NGN",
//                payment_type: "redirect",
//                redirect_url: "https://localhost:2222/hostels/RaveRedirect",
//                meta: {
//                    custom_fields: [
//                        {
//                            display_name: "Description",
//                            variable_name: "description",
//                            value: "Rave Checkout"
//                        }
//                    ]
//                },
//                customer: {
//                    email: email,
//                }
//            };

//            // Call the FlutterwaveCheckout function with the payment data
//            FlutterwaveCheckout(paymentData);
//        })
//        .catch(error => {
//            console.error('Failed to retrieve Flutterwave API key:', error);
//        });
//}

function makePayment() {
    const publicKey = 'FLWPUBK_TEST-3f35866dc8566ccf6b5b8a468536f069-X'//document.querySelector('input[name="public_key"]').value;
    const email = document.getElementById('email').value;
    console.log(email, publicKey)
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
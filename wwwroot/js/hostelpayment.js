const ravePaymentForm = document.getElementById('ravePaymentForm');
ravePaymentForm.addEventListener("submit", payWithRave, false);

function payWithRave(e) {
    e.preventDefault(); // Prevent default form submission

    fetch('/wallets/GetRavePaymentKey')
        .then(response => response.json())
        .then(data => {
            const paymentData = {
                public_key: data,
                tx_ref: document.getElementById("ref").value,
                amount: document.getElementById("amount").value * 100,
                currency: "NGN",
                payment_options: "card, banktransfer, ussd",
                redirect_url: "https://edouniversity.edu.ng",

               
                //meta: {
                //    consumer_id: 23,
                //    consumer_mac: "92a3-912ba-1192a",
                //},
                customer: {
                    email: document.getElementById("customer[email]").value,
                },
                customizations: {
                    title: "Edo State University Uzairue",
                    description: "Accommodation payment",
                    logo: "https://old.edouniversity.edu.ng/uploads/settings/logo.png",
                },
            };

            // Call the FlutterwaveCheckout function
            //payWithRave(paymentData);
        });
};


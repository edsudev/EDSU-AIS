    document.addEventListener("DOMContentLoaded", function () {
        const startPaymentButton = document.getElementById("start-payment-button");

        startPaymentButton.addEventListener("click", function () {
            // Get form data
            const form = document.getElementById("payment-form");
            const formData = new FormData(form);

            // Construct the payment request data
            const paymentData = {
                public_key: "FLWPUBK_TEST-3f35866dc8566ccf6b5b8a468536f069-X",
                tx_ref: formData.get("tx_ref"),
                amount: parseFloat(formData.get("amount")),
                currency: "NGN",
                payment_options: "card, banktransfer, ussd",
                redirect_url: "https://edouniversity.edu.ng",
                //meta: {
                //    consumer_id: 23,
                //    consumer_mac: "92a3-912ba-1192a",
                //},
                //customer: {
                //    email: formData.get("customer[email]"),
                //    phone_number: "08102909304",
                //    name: "Rose DeWitt Bukater",
                //},
                customizations: {
                    title: "Edo State University Uzairue",
                    description: "Accommodation payment",
                    logo: "https://old.edouniversity.edu.ng/uploads/settings/logo.png",
                },
            };

            // Call the FlutterwaveCheckout function
            FlutterwaveCheckout(paymentData);
        });
    });


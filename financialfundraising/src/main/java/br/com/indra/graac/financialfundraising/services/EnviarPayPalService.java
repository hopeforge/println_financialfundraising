package br.com.indra.graac.financialfundraising.services;

import java.util.ArrayList;
import java.util.List;
import org.springframework.stereotype.Service;
import com.paypal.api.payments.Amount;
import com.paypal.api.payments.Payer;
import com.paypal.api.payments.Payment;
import com.paypal.api.payments.PaymentExecution;
import com.paypal.api.payments.RedirectUrls;
import com.paypal.api.payments.Transaction;
import com.paypal.base.rest.APIContext;
import com.paypal.base.rest.PayPalRESTException;
import com.paypal.api.payments.Authorization;

@Service
public class EnviarPayPalService {

	private static final String CLIENT_ID = "";
	private static final String CLIENT_SECRET = "";
	private static final String SANDBOX = "sandbox";

	public void enviarPayPal(String valorDoacao) {

		Amount amount = new Amount();
		amount.setCurrency("BRL");
		amount.setTotal(valorDoacao);

		Transaction transaction = new Transaction();
		transaction.setDescription("financialfundraising --> Enviando doação para o paypal,graac!");
		transaction.setAmount(amount);

		List<Transaction> lt = new ArrayList<Transaction>();
		lt.add(transaction);

		Payer payer = new Payer();
		payer.setPaymentMethod("paypal");

		RedirectUrls ru = new RedirectUrls();
		ru.setCancelUrl("http://localhost:3000/crunchifyCancel");
		ru.setReturnUrl("http://localhost:3000/crunchifyReturn");

		Payment payment = new Payment();
		payment.setIntent("sale");
		payment.setPayer(payer);
		payment.setTransactions(lt);
		payment.setRedirectUrls(ru);

		APIContext apiContext = new APIContext(CLIENT_ID, CLIENT_SECRET, SANDBOX);

		try {
			Payment createPayment = payment.create(apiContext);
			payment.setId(createPayment.getId());

			PaymentExecution paymentExecution = new PaymentExecution();

			paymentExecution.setPayerId(payment.getId());

			Payment createdAuth = payment.execute(apiContext, paymentExecution);

			Authorization authorization = createdAuth.getTransactions().get(0).getRelatedResources().get(0)
					.getAuthorization();

			System.out.println(">>> id authorization .." + authorization.getId());

		} catch (PayPalRESTException e) {
			e.printStackTrace();// colocar log4j
		}

	}
}
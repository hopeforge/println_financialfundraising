package br.com.indra.graac.financialfundraising.services;

import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.mail.MailException;
import org.springframework.mail.SimpleMailMessage;
import org.springframework.mail.javamail.JavaMailSender;
import org.springframework.stereotype.Service;

import br.com.indra.graac.financialfundraising.components.DestinatarioEmail;

@Service
public class EnviarEmailService {

	private JavaMailSender mailSender;
	
	@Autowired
	public EnviarEmailService(JavaMailSender mailSender) {
		this.mailSender = mailSender;
	}
	
	public void enviarEmail(DestinatarioEmail destinatario) throws MailException {
		
		SimpleMailMessage mail = new SimpleMailMessage();
		mail.setTo(destinatario.getEmail());
		mail.setSubject("Doação Recebida");
		mail.setText("Sr(a). "+destinatario.getNome() +"Recebemos sua doação no valor de R$" +destinatario.getValorDoacao() + ",Para consultar as suas doações consulte <link> ... com usuario " + destinatario.getEmail() +"e a senha" + destinatario.getSenha() +" GRAAC agradece.");

		mailSender.send(mail);
	}
	
}
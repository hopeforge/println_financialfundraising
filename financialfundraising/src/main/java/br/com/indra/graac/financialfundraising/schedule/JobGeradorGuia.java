package br.com.indra.graac.financialfundraising.schedule;

import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.context.annotation.Configuration;
import org.springframework.scheduling.annotation.EnableScheduling;
import org.springframework.scheduling.annotation.Scheduled;
import br.com.indra.graac.financialfundraising.services.EnviarPayPalService;

@Configuration
@EnableScheduling
public class JobGeradorGuia {

	@Autowired
	private EnviarPayPalService enviarPayPalService;

	 //@Scheduled(fixedDelay = 1000)
	@Scheduled(cron = "0 0 0 * * *") // executa meia noite todos os dias
	public void gerarGuiaDiaria() {
		enviarPayPalService.enviarPayPal(null);
	}

}

package br.com.indra.graac.financialfundraising.resources;

import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.PostMapping;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RequestMethod;
import org.springframework.web.bind.annotation.RequestParam;
import org.springframework.web.bind.annotation.ResponseBody;
import org.springframework.web.bind.annotation.RestController;

import br.com.indra.graac.financialfundraising.entity.ProcessaRepasse;
import br.com.indra.graac.financialfundraising.repository.ProcessaRepasseRepository;

@RestController
@RequestMapping(value="/api")
public class LojaResource {

	
	@Autowired
	private ProcessaRepasseRepository prr;
	
	@PostMapping(value="/loja",produces="application/json",params= {"idLoja"})
	public @ResponseBody float retornaValor(@RequestParam("idLoja")int idLoja) {
		
		
		ProcessaRepasse vw = prr.findValorById(idLoja);
		return vw.getValor();
		
	}
	
	
}

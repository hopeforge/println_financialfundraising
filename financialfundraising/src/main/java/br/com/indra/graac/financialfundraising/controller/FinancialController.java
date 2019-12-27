package br.com.indra.graac.financialfundraising.controller;

import java.util.ArrayList;
import java.util.Date;
import java.util.List;

import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.web.bind.annotation.PostMapping;
import org.springframework.web.bind.annotation.RequestBody;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RestController;

import br.com.indra.graac.financialfundraising.components.DestinatarioEmail;
import br.com.indra.graac.financialfundraising.entity.Doacoes;
import br.com.indra.graac.financialfundraising.entity.Doador;
import br.com.indra.graac.financialfundraising.repository.DoadorRepository;
import br.com.indra.graac.financialfundraising.request.DoacaoRequest;
import br.com.indra.graac.financialfundraising.response.DoacaoResponse;
import br.com.indra.graac.financialfundraising.services.EnviarEmailService;
import br.com.indra.graac.financialfundraising.util.GeradorSenhaUtil;


@RestController
@RequestMapping(path = "/financialfundraising")
public class FinancialController {

	@Autowired
	private EnviarEmailService enviarEmailService;
	@Autowired
	private DoadorRepository doadorRepository;
	
	//localhost:8080/financialfundraising/
	@PostMapping(path= "/", consumes = "application/json", produces = "application/json")
	public DoacaoResponse fundraising(@RequestBody DoacaoRequest doacaoRequest) {
				
		checkNovoDoador(doacaoRequest);
		return new DoacaoResponse();
	}
	
	private void checkNovoDoador(DoacaoRequest doacaoRequest) {
		Doador  doador = doadorRepository.findByCPFEmail(doacaoRequest.getCpf(), doacaoRequest.getEmail());
		if(doador == null) {
			doador = new Doador();
			doador.setCpf(doacaoRequest.getCpf());
			doador.setEmail(doacaoRequest.getEmail());
			doador.setSenha(GeradorSenhaUtil.gerarSenha());
			doador.setNomeDoador(doacaoRequest.getNome());
			
			doador.setIdDoador(0);	
		}
		
		Doacoes doacoes = new Doacoes();
		doacoes.setDataDoacao(new Date());
		doacoes.setIdDoacao(0);
		doacoes.setIdDoador(doador);
		doacoes.setValor(new Double(0.00));
		doacoes.setNumPedido("5");
		doacoes.setIdLoja(4);
		
		if(doacaoRequest.getValorDoacao() != null && !"".equals(doacaoRequest.getValorDoacao())) {
			doacoes.setValor(new Double(doacaoRequest.getValorDoacao()));	
		}
		
		List<Doacoes> list = new ArrayList<Doacoes>();
		list.add(doacoes);
		doador.setDoacoesList(list);
		
		doadorRepository.save(doador);	
		enviarEmailService.enviarEmail(new DestinatarioEmail(doacaoRequest.getNome(),doacaoRequest.getValorDoacao(),doador.getEmail(),doador.getSenha()));
	}
}
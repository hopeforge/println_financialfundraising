package br.com.indra.graac.financialfundraising.controller;

import java.util.ArrayList;
import java.util.List;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.PathVariable;
import org.springframework.web.bind.annotation.PostMapping;
import org.springframework.web.bind.annotation.RequestBody;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RestController;
import br.com.indra.graac.financialfundraising.entity.Doacoes;
import br.com.indra.graac.financialfundraising.entity.Doador;
import br.com.indra.graac.financialfundraising.repository.DoadorRepository;
import br.com.indra.graac.financialfundraising.request.LoginRequest;
import br.com.indra.graac.financialfundraising.response.LoginResponse;
import br.com.indra.graac.financialfundraising.response.DoacoesResponse;

@RequestMapping(path = "/doacoes/")
@RestController
public class HistoricoController {

	@Autowired
	private DoadorRepository doadorRepository;

	@GetMapping("{cpf}")
	public LoginResponse getAllDoacoesForUser(@PathVariable String cpf) {
		LoginResponse lr = new LoginResponse();
		if (cpf != null && !"".equals(cpf)) {
			Doador doador = doadorRepository.findByCpf(cpf);
			if (doador != null) {
				lr.setCpf(doador.getCpf());
				lr.setEmail(doador.getEmail());
				lr.setNomeDoador(doador.getNomeDoador());
				lr.setDoacoes(getList(doador));
			}
		}
		return lr;
	}

	@PostMapping(path = "/", consumes = "application/json", produces = "application/json")
	public LoginResponse login(@RequestBody LoginRequest loginRequest) {
		Doador doador = doadorRepository.findByCPFSenha(loginRequest.getCpf(), loginRequest.getSenha());
		LoginResponse lr = new LoginResponse();
		if (doador != null) {
			lr = loginOK(doador);
		}
		return lr;
	}

	private LoginResponse loginOK(Doador doador) {

		LoginResponse lr = new LoginResponse();
		lr.setCpf(doador.getCpf());
		lr.setEmail(doador.getEmail());
		lr.setNomeDoador(doador.getNomeDoador());

		lr.setDoacoes(getList(doador));

		return lr;
	}

	private List<DoacoesResponse> getList(Doador doad) {

		List<DoacoesResponse> ldr = new ArrayList<DoacoesResponse>();
		for (Doacoes doacoes : doad.getDoacoesList()) {
			DoacoesResponse dr = new DoacoesResponse();
			dr.setDataDoacao(doacoes.getDataDoacao());
			dr.setNumPedido(doacoes.getNumPedido());
			dr.setValor(dr.getValor());
			ldr.add(dr);
		}
		return ldr;
	}
}
package br.com.indra.graac.financialfundraising.repository;

import org.springframework.data.jpa.repository.JpaRepository;
import org.springframework.stereotype.Repository;
import br.com.indra.graac.financialfundraising.entity.Doador;

@Repository
public interface DoadorRepository extends JpaRepository<Doador,Integer>{

	public Doador findByIdDoador(Integer idDoador);
	public Doador findByNomeDoador(String nomeDoador);
	public Doador findByCpf(String cpf);
	public Doador findByEmail(String email);
	public Doador findByCPFEmail(String cpf , String email);
	public Doador findByCPFSenha(String cpf , String senha);
	
}


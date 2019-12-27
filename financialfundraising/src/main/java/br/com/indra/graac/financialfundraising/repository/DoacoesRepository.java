package br.com.indra.graac.financialfundraising.repository;

import org.springframework.data.jpa.repository.JpaRepository;
import org.springframework.stereotype.Repository;
import br.com.indra.graac.financialfundraising.entity.Doacoes;

@Repository
public interface  DoacoesRepository extends JpaRepository<Doacoes,Integer>{

}

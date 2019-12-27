package br.com.indra.graac.financialfundraising.repository;

import org.springframework.data.jpa.repository.JpaRepository;
import org.springframework.stereotype.Repository;
import br.com.indra.graac.financialfundraising.entity.Loja;

@Repository
public interface LojaRepository extends JpaRepository<Loja,Integer>{

}

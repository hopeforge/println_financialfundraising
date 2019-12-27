package br.com.indra.graac.financialfundraising.repository;

import org.springframework.data.jpa.repository.JpaRepository;
import org.springframework.data.jpa.repository.Query;
import org.springframework.data.repository.query.Param;
import org.springframework.stereotype.Repository;

import br.com.indra.graac.financialfundraising.entity.ProcessaRepasse;
@Repository
public interface ProcessaRepasseRepository extends JpaRepository<ProcessaRepasse, Integer> {
	@Query(value="SELECT * FROM VW_PROCESSA_REPASSE vw WHERE ID_LOJA=(:id)", nativeQuery=true)
	public ProcessaRepasse findValorById(@Param("id") int id);
	
}
